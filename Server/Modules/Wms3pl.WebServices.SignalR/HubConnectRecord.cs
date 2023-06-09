using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.SignalR
{
	public class HubConnect
	{
		public string ConnectId { get; set; }
		public string UserName { get; set; }
		public string HostName { get; set; }
		public DateTime? UnLockTime { get; set; }
		public string CRT_STAFF { get; set; }
		public string CRT_NAME { get; set; }
	}
	public class HubConnectRecord
	{
		private readonly Dictionary<string, HashSet<HubConnect>> _connections = new Dictionary<string, HashSet<HubConnect>>();

		/// <summary>
		/// 新增群組連線資訊
		/// </summary>
		/// <param name="groupName">群組名稱</param>
		/// <param name="hubConnect">連線資訊</param>
		public void Add(string groupName,HubConnect hubConnect, HubRecordMode hubRecordMode,string schema)
		{
			if (hubRecordMode == HubRecordMode.Memory)
			{
				lock (_connections)
				{

					HashSet<HubConnect> hubConnects;
					if (!_connections.TryGetValue(groupName, out hubConnects))
					{
						hubConnects = new HashSet<HubConnect>();
						_connections.Add(groupName, hubConnects);
					}
					lock (hubConnects)
						hubConnects.Add(hubConnect);
				}
			}
			else
			{
				var wmsTransation = new WmsTransaction();
				var f0070Repo = new F0070Repository(schema, wmsTransation);
				f0070Repo.Add(new F0070
				{
					CONNECTID = hubConnect.ConnectId,
					USERNAME = hubConnect.UserName,
					HOSTNAME = hubConnect.HostName,
					GROUPNAME = groupName,
					CRT_DATE = DateTime.Now,
					CRT_STAFF = hubConnect.CRT_STAFF,
					CRT_NAME = hubConnect.CRT_NAME
				});
				wmsTransation.Complete();
			}
		}
		/// <summary>
		/// 移除群組連線資訊
		/// </summary>
		/// <param name="groupName">群組名稱</param>
		/// <param name="connectId">連線資訊</param>
		public void Remove(string groupName,string connectId, HubRecordMode hubRecordMode,string schema)
		{
			if (hubRecordMode == HubRecordMode.Memory)
			{
				lock (_connections)
				{
					HashSet<HubConnect> hubConnects;
					if (!_connections.TryGetValue(groupName, out hubConnects))
						return;

					lock (hubConnects)
					{
						var item = hubConnects.FirstOrDefault(o => o.ConnectId == connectId);
						if (item != null)
						{
							hubConnects.Remove(item);
							if (hubConnects.Count == 0)
								_connections.Remove(groupName);
						}
					}
				}
			}
			else
			{
				var wmsTransation = new WmsTransaction();
				var f0070Repo = new F0070Repository(schema, wmsTransation);
				f0070Repo.AsForUpdate().Delete(o => o.GROUPNAME == groupName && o.CONNECTID == connectId);
				wmsTransation.Complete();
			}
		}
		/// <summary>
		/// 移除群組此帳號所有連線資訊
		/// </summary>
		/// <param name="groupName">群組名稱</param>
		/// <param name="connectId">帳號</param>
		public void RemoveAllByName(string groupName, string userName, HubRecordMode hubRecordMode,string schema)
		{
			if (hubRecordMode == HubRecordMode.Memory)
			{
				lock (_connections)
				{
					HashSet<HubConnect> hubConnects;
					if (!_connections.TryGetValue(groupName, out hubConnects))
						return;

					lock (hubConnects)
					{
						hubConnects.RemoveWhere(o => o.UserName == userName);
						if (hubConnects.Count == 0)
							_connections.Remove(groupName);
					}
				}
			}
			else
			{
				var wmsTransation = new WmsTransaction();
				var f0070Repo = new F0070Repository(schema, wmsTransation);
				f0070Repo.AsForUpdate().RemoveAllByGroupNameAndUserName(groupName,userName);
				wmsTransation.Complete();
			}
		}
		public bool IsExist(string groupName,string connectId, HubRecordMode hubRecordMode,string schema)
		{
				return Find(groupName, connectId, hubRecordMode, schema) != null;
		}
		/// <summary>
		/// 依連線Id取得使用者資訊
		/// </summary>
		/// <param name="groupName">群組名稱</param>
		/// <param name="connectId">連線ID</param>
		/// <returns></returns>
		public HubConnect Find(string groupName,string connectId, HubRecordMode hubRecordMode,string schema)
		{
			if (hubRecordMode == HubRecordMode.Memory)
			{
				lock (_connections)
				{
					HashSet<HubConnect> hubConnects;
					if (!_connections.TryGetValue(groupName, out hubConnects))
						return null;

					lock (hubConnects)
					{
						return hubConnects.FirstOrDefault(o => o.ConnectId == connectId);
					}
				}
			}
			else
			{
				var f0070Repo = new F0070Repository(schema);
				var item = f0070Repo.Get(groupName,connectId);
				if(item!=null)
				{
					return new HubConnect
					{
						ConnectId = item.CONNECTID,
						UserName = item.USERNAME,
						HostName = item.HOSTNAME,
						UnLockTime = item.UNLOCKTIME,
						CRT_STAFF = item.CRT_STAFF,
						CRT_NAME = item.CRT_NAME
					};
				}
				return null;
			}
		}
		/// <summary>
		/// 依帳號尋找同帳號非目前連線的使用者的其他使用者
		/// </summary>
		/// <param name="groupName">群組名稱</param>
		/// <param name="userName">帳號</param>
		/// <param name="excludeConnectId">排除的使用者連線Id</param>
		/// <returns></returns>
		public IEnumerable<HubConnect> FindNameByExcludeCurrentConnectId(string groupName, string userName,string excludeConnectId, HubRecordMode hubRecordMode,string schema)
		{
			if (hubRecordMode == HubRecordMode.Memory)
			{
				lock (_connections)
				{
					HashSet<HubConnect> hubConnects;
					if (!_connections.TryGetValue(groupName, out hubConnects))
						return null;

					lock (hubConnects)
					{
						return hubConnects.Where(o => o.ConnectId != excludeConnectId && o.UserName == userName);
					}
				}
			}
			else
			{
				var f0070Repo = new F0070Repository(schema);
				return f0070Repo.FindNameByExcludeCurrentConnectId(groupName, userName, excludeConnectId).Select(o => new HubConnect
				{
					ConnectId = o.CONNECTID,
					UserName = o.USERNAME,
					HostName = o.HOSTNAME,
					UnLockTime = o.UNLOCKTIME,
					CRT_STAFF = o.CRT_STAFF,
					CRT_NAME = o.CRT_NAME
				}).AsEnumerable();
			}
		}

		public void UpdateUnLockDate(string groupName,string connectId,DateTime? date, HubRecordMode hubRecordMode,string schema)
		{
			var item = HubConnectionStorage.HubConnectedRecord.Find(groupName, connectId, hubRecordMode, schema);
			if (item != null)
			{
				if (hubRecordMode == HubRecordMode.Memory)
					item.UnLockTime = date;
				else
				{
					var wmsTransation = new WmsTransaction();
					var f0070Repo = new F0070Repository(schema, wmsTransation);
					f0070Repo.AsForUpdate().UpdateUnLockDate(groupName, connectId, date);
					wmsTransation.Complete();
				}
			}
		}

		public List<HubConnect> GetAllHubConnect(string groupName, HubRecordMode hubRecordMode,string schema)
		{
			if (hubRecordMode == HubRecordMode.Memory)
				return _connections[groupName].ToList();
			else
			{
				var f0070Repo = new F0070Repository(schema);
				return f0070Repo.GetAllByGroup(groupName).Select(x => new HubConnect
				{
					ConnectId = x.CONNECTID,
					UserName = x.USERNAME,
					HostName = x.HOSTNAME,
					UnLockTime = x.UNLOCKTIME,
					CRT_STAFF = x.CRT_STAFF,
					CRT_NAME = x.CRT_NAME
				}).ToList();
			}
		}
	}
}
