using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.DataCommon.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
  public partial class P81Service
  {
    private WmsTransaction _wmsTransation;
    public P81Service(WmsTransaction wmsTransation = null)
    {
      _wmsTransation = wmsTransation;
    }

    /// <summary>
    /// ���o�T���N�X�w�q���
    /// </summary>
    /// <returns></returns>
    private List<MsgModel> GetMsgData()
    {
      // �ثe���g�����
      return new List<MsgModel>
      { 
        #region 00�@�Χ@�~
        new MsgModel{ MsgCode = "10001", MsgContent = "���o��Ʀ��\" },
        new MsgModel{ MsgCode = "10002", MsgContent = "��s���\" },
        new MsgModel{ MsgCode = "10003", MsgContent = "�U�[���\" },
        new MsgModel{ MsgCode = "10004", MsgContent = "�W�[���\" },
        new MsgModel{ MsgCode = "10005", MsgContent = "���榨�\" },
        new MsgModel{ MsgCode = "10006", MsgContent = "�ӫ~��J���\�A�Ц^���e��" },
        new MsgModel{ MsgCode = "10007", MsgContent = "�Ĥ@�c�e���i�����\" },
        new MsgModel{ MsgCode = "10008", MsgContent = "���e���X�����\" },
        new MsgModel{ MsgCode = "10009", MsgContent = "�z�f�����A�Ш�{0}" },
        new MsgModel{ MsgCode = "10010", MsgContent = "���禨�\" },
        new MsgModel{ MsgCode = "10011", MsgContent = "����q�L" },
        new MsgModel{ MsgCode = "10012", MsgContent = "�s�e���в��ܲ��`�ϡC��e�����ӫ~���礣�q�L�A�]�в����`��" },
        new MsgModel{ MsgCode = "10013", MsgContent = "���ӫ~���礣�q�L�A���e���������ӫ~�|������" },
        new MsgModel{ MsgCode = "10014", MsgContent = "�e���w����/��������" },
        new MsgModel{ MsgCode = "10015", MsgContent = "���ӫ~����q�L�A���e�������祢�Ѫ��ӫ~�A�в����`��" },
        new MsgModel{ MsgCode = "10016", MsgContent = "���ӫ~�אּ��������A���e���������祢�Ѫ��ӫ~�A�в��ܲ��`��" },
        new MsgModel{ MsgCode = "10017", MsgContent = "���ӫ~�אּ��������C���e�������Ҧ��ӫ~���w��������" },
        new MsgModel{ MsgCode = "10018", MsgContent = "�ӫ~�w��J�s�e���C�]�����礣�q�L�A�бN�s�e�����ʨ첧�`��" },
        new MsgModel{ MsgCode = "10019", MsgContent = "���礣�q�L�A�бN�e�����ʨ첧�`��" },
        new MsgModel{ MsgCode = "10020", MsgContent = "�X�f�c�i�����\" },
        new MsgModel{ MsgCode = "10021", MsgContent = "���t�h�X�f��X�����\!" },
        new MsgModel{ MsgCode = "10022",MsgContent = "�������Ȧ��\"},
        new MsgModel{ MsgCode = "10023",MsgContent = "�����Ȧ��\"},
        new MsgModel{ MsgCode = "10024",MsgContent = "�����ɳf���\"},
        new MsgModel{ MsgCode = "10025",MsgContent = "�ӫ~�w��J�s�e���C�]�����礣�q�L�A�бN�s�e�����ʨ첧�`�ϡA��e���w�L����ӫ~�A�Ц^��"},

        new MsgModel{ MsgCode = "20050", MsgContent = "{0}���o����" },
        new MsgModel{ MsgCode = "20051", MsgContent = "���o��ƥ���" },
        new MsgModel{ MsgCode = "20052", MsgContent = "��s����" },
        new MsgModel{ MsgCode = "20053", MsgContent = "�L���x��" },
        new MsgModel{ MsgCode = "20054", MsgContent = "�W�[�x��ᵲ�i" },
        new MsgModel{ MsgCode = "20055", MsgContent = "�z�L���x���v��" },
        new MsgModel{ MsgCode = "20056", MsgContent = "�W�[�x��D���f�D�x��" },
        new MsgModel{ MsgCode = "20057", MsgContent = "���ӫ~�����\�V��" },
        new MsgModel{ MsgCode = "20058", MsgContent = "���ӫ~�����\�V�~" },
        new MsgModel{ MsgCode = "20059", MsgContent = "�ƶq�����j��0" },
        new MsgModel{ MsgCode = "20060", MsgContent = "�w�W�L�Ѿl���W�[��" },
        new MsgModel{ MsgCode = "20061", MsgContent = "����ڤw�Q��L�@�~�H���ϥ�" },
        new MsgModel{ MsgCode = "20062", MsgContent = "�U�[�x��ᵲ�X" },
        new MsgModel{ MsgCode = "20063", MsgContent = "�w�W�L�Ѿl���U�[��" },
        new MsgModel{ MsgCode = "20064", MsgContent = "��ڤ��s�b" },
        new MsgModel{ MsgCode = "20065", MsgContent = "��ڤw�R��" },
        new MsgModel{ MsgCode = "20066", MsgContent = "��ڤw����" },
        new MsgModel{ MsgCode = "20067", MsgContent = "��ک��Ӥw�����U�[�A�U�[����" },
        new MsgModel{ MsgCode = "20068", MsgContent = "��ک��Ӥw�����W�[�A�W�[����" },
        new MsgModel{ MsgCode = "20069", MsgContent = "�ǤJ���Ѽ����ҥ��ѡC" },
        new MsgModel{ MsgCode = "20070", MsgContent = "�ثe����ڪ��A�D���@�~�i����C" },
        new MsgModel{ MsgCode = "20071", MsgContent = "�x��{0}�żh{1}���ŦX�ӫ~{2}�żh{3}" },
        new MsgModel{ MsgCode = "20072", MsgContent = "�Ӱӫ~{0}���żh��Ƥ��s�b" },
        new MsgModel{ MsgCode = "20073", MsgContent = "���x��{0}���żh��Ƥ��s�b" },
        new MsgModel{ MsgCode = "20074", MsgContent = "���x��w�g�s���L�f�D���ӫ~" },
        new MsgModel{ MsgCode = "30001", MsgContent = "�ӳ�ڤw���H��(%s)�i��@�~���A�аݬO�_�n�󴫧@�~�H��?" },
        #endregion

        #region 01�v���@�~
        new MsgModel{ MsgCode = "10101", MsgContent = "�n�J���\�C" },
        new MsgModel{ MsgCode = "10102", MsgContent = "�n�X���\" },
        new MsgModel{ MsgCode = "20151", MsgContent = "�z��J����Ʀ��~�A�Э��s��J�C" },
        new MsgModel{ MsgCode = "20152", MsgContent = "�n�X����" },
        new MsgModel{ MsgCode = "20153", MsgContent = "���s�����A�ФU���̷s����" },
        new MsgModel{ MsgCode = "20154", MsgContent = "���b���w�b��L�˸m�n�J" },
        #endregion

        #region 02�D�ɧ@�~
        new MsgModel{ MsgCode = "20251", MsgContent = "���o�ӫ~��ƥ���" },
        new MsgModel{ MsgCode = "20252", MsgContent = "���o�ӫ~�Ǹ���ƥ���" },
        #endregion

        #region 03�ռ��@�~
        new MsgModel{ MsgCode = "20351", MsgContent = "���o�ռ���ڸ�ƥ���" },
        new MsgModel{ MsgCode = "20352", MsgContent = "���o�ռ����Ӹ�ƥ���" },
        new MsgModel{ MsgCode = "20353", MsgContent = "���e��{0}�L�k���������ռ��渹" },
        new MsgModel{ MsgCode = "20354", MsgContent = "���x��{0}���s�b" },
        new MsgModel{ MsgCode = "20355", MsgContent = "���x��{0}���s�b��{1}" },
        new MsgModel{ MsgCode = "20356", MsgContent = "���Ǹ�{0}�L�k���������ռ��渹" },
        #endregion

        #region 04 �X�f�@�~
        new MsgModel{ MsgCode = "20451", MsgContent = "���o�z�f��ڸ�ƥ���" },
        new MsgModel{ MsgCode = "20452", MsgContent = "���o�z�f���Ӹ�ƥ���" },
        new MsgModel{ MsgCode = "20453", MsgContent = "�z�f�ƶq�����j�󵥩�0" },
        new MsgModel{ MsgCode = "20454", MsgContent = "�ʳf�ƶq�����j��0" },
        new MsgModel{ MsgCode = "20455", MsgContent = "�z�f�ƶq�w�W�L�Ѿl���z��" },
        new MsgModel{ MsgCode = "20456", MsgContent = "�ʳf�ƶq�w�W�L�Ѿl���z��" },
        new MsgModel{ MsgCode = "20457", MsgContent = "�z�f���Ӥw�Q[{0}]�z�f����" },
        new MsgModel{ MsgCode = "20458", MsgContent = "�z�f���Ӥw����" },
        new MsgModel{ MsgCode = "20459", MsgContent = "�z�f���Ӥ��s�b" },
        new MsgModel{ MsgCode = "20460", MsgContent = "�q��w�����A�����j�w�P�z�f" },
        #endregion

        #region 05 �w�s�@�~
        new MsgModel{ MsgCode = "20551", MsgContent = "���o�w�s��ƥ���" },
        #endregion

        #region 06 �L�I�@�~
        new MsgModel{ MsgCode = "20651", MsgContent = "���o�L�I��ڸ�ƥ���" },
        new MsgModel{ MsgCode = "20652", MsgContent = "���o�L�I���Ӹ�ƥ���" },
        new MsgModel{ MsgCode = "20601", MsgContent = "�L�I��ƽT�{���\" },
        new MsgModel{ MsgCode = "20602", MsgContent = "�L�I��Ʒs�W���\" },
        new MsgModel{ MsgCode = "20653", MsgContent = "�L�I�ƶq�����p��0" },
        new MsgModel{ MsgCode = "20654", MsgContent = "�L�I�渹���s�b" },
        new MsgModel{ MsgCode = "20655", MsgContent = "�~�����s�b" },
        new MsgModel{ MsgCode = "20656", MsgContent = "�x�줣�s�b" },
        new MsgModel{ MsgCode = "20657", MsgContent = "�L�I��Ƥ��s�b" },
        new MsgModel{ MsgCode = "20658", MsgContent = "�L�I��Ƥw�s�b  ���i�s�W" },
        new MsgModel{ MsgCode = "21951", MsgContent = "�Ǹ����s�b"},
        new MsgModel{ MsgCode = "20659", MsgContent = "���x�쪺�ܧO���s�b" },
        new MsgModel{ MsgCode = "20660", MsgContent = "���x�쪺�ܧO���i���۰ʭ�" },
        new MsgModel{ MsgCode = "20661", MsgContent = "�Ǹ��j�x��ӫ~�ƶq����W�L1" },
        #endregion

        #region 08 �Ǹ��@�~
        new MsgModel{ MsgCode = "20851", MsgContent = "�~���B�Ǹ��ܤ@����" },
        #endregion

        #region �h���@�~
        new MsgModel{ MsgCode = "20701", MsgContent = "�h����إߦ��\" },
        new MsgModel{ MsgCode = "20751", MsgContent = "���o�d���x���ƥ���" },
        new MsgModel{ MsgCode = "20752", MsgContent = "���o�d�߰ӫ~�x���ƥ���" },
        new MsgModel{ MsgCode = "20753", MsgContent = "�h���ƶq���j��0" },
        new MsgModel{ MsgCode = "20754", MsgContent = "�w�s�ƶq{0}�֩�h���ƶq{1}�A�h������" },
        new MsgModel{ MsgCode = "20755", MsgContent = "�x��{0}���s�b�A�h������" },
        new MsgModel{ MsgCode = "20756", MsgContent = "�h����إߥ��ѡA�ӫ~���Q�h���A��]��{0}" },
        new MsgModel{ MsgCode = "20757", MsgContent = "�����۰ʭ��x��A�Ш�J�~��" },
        new MsgModel{ MsgCode = "20758", MsgContent = "�W�[�x��{0}�|���]�w�x�ϡA���i�H�W�[" },
        new MsgModel{ MsgCode = "20759", MsgContent = "�x��{0}�|���]�w�x��" },
        new MsgModel{ MsgCode = "20760", MsgContent = "�ӷ��x��{0}�w�ᵲ" },
        #endregion

        #region 10 ��w�ռ��@�~
        new MsgModel{ MsgCode = "21001", MsgContent = "�e���s�����o����" },
        new MsgModel{ MsgCode = "21002", MsgContent = "�e���s�����s�b" },
        new MsgModel{ MsgCode = "21003", MsgContent = "�e�����õL�ӫ~�s�b" },
        new MsgModel{ MsgCode = "21004", MsgContent = "�Ӯe���S���i�ܳ�" },
        new MsgModel{ MsgCode = "21005", MsgContent = "�i�ܳ檬�A���~�I{0}" },
        new MsgModel{ MsgCode = "21006", MsgContent = "�~�����X/�Ǹ����o����" },
        new MsgModel{ MsgCode = "21007", MsgContent = "�ӱ��X���s�b�~��/�i�ܧǸ�" },
        new MsgModel{ MsgCode = "21008", MsgContent = "���ӫ~���ݩ󦹮e��" },
        new MsgModel{ MsgCode = "21009", MsgContent = "�䤣���w�ռ��i�f�ܧO�s��" },
        new MsgModel{ MsgCode = "21010", MsgContent = "�䤣���w�ռ��i�f�ܧO�W��" },
        new MsgModel{ MsgCode = "21011", MsgContent = "�ӫ~�Ǹ��ˮ֦��~�I{0}" },
        new MsgModel{ MsgCode = "21012", MsgContent = "���Ǹ����ݩ󦹮e��" },
        new MsgModel{ MsgCode = "21013", MsgContent = "�Ӯe���w�禬�L" },
        new MsgModel{ MsgCode = "21014", MsgContent = "�x��s�����o����" },
        new MsgModel{ MsgCode = "21015", MsgContent = "���x��ä����\�i���w�ռ��W�[�A�п�J���T�x��" },
        new MsgModel{ MsgCode = "21016", MsgContent = "�x��s�����s�b" },
        new MsgModel{ MsgCode = "21017", MsgContent = "���x��w�g����L�f�D���ӫ~�A�дM���L�x��" },
        new MsgModel{ MsgCode = "21018", MsgContent = "���x��Q�ᵲ�A�����\�i�f�W�[" },
        new MsgModel{ MsgCode = "21019", MsgContent = "���e���w�g���ƤW�[�����A�L�k�d��" },
        new MsgModel{ MsgCode = "21020", MsgContent = "���e���|���禬�A�Х��i��i���禬" },
        new MsgModel{ MsgCode = "21021", MsgContent = "�妸�W�[���ѡA��]��{0}" },
        new MsgModel{ MsgCode = "21022", MsgContent = "�s�ؽռ��楢�ѡA��]��{0}" },
        new MsgModel{ MsgCode = "21023", MsgContent = "����w�i�f�e��{0}�t�Υ��b�B�z��"},
        new MsgModel{ MsgCode = "21024", MsgContent = "�W�[�ܧO���s�b"},
        #endregion

        #region 11 ���f�X�J��
        new MsgModel{ MsgCode = "21101", MsgContent = "���f�����o����" },
        new MsgModel{ MsgCode = "21102", MsgContent = "���e���L��������ڸ��X" },
        new MsgModel{ MsgCode = "21103", MsgContent = "�䤣��X�f�妸������" },
        new MsgModel{ MsgCode = "21104", MsgContent = "���e�����춰�f��{0}���f" },
        new MsgModel{ MsgCode = "21105", MsgContent = "���e�����ݶi���f���A�Ш�{0}" },
        new MsgModel{ MsgCode = "21106", MsgContent = "���X�f�泣�w����A���i�A�i��" },
        new MsgModel{ MsgCode = "21107", MsgContent = "���f��{0}�x������{1}�w��" },
        new MsgModel{ MsgCode = "21108", MsgContent = "���f���s�����s�b" },
        new MsgModel{ MsgCode = "21109", MsgContent = "�бN�ӫ~���Ĥ@�c����A�ĤG�c�Ц^��" },
        new MsgModel{ MsgCode = "21110", MsgContent = "�бN�e������x��W�A�è�J�x����X" },
        new MsgModel{ MsgCode = "21111", MsgContent = "�䤣�춰�f��Ƭ[" },
        new MsgModel{ MsgCode = "21112", MsgContent = "�x����X���o����" },
        new MsgModel{ MsgCode = "21113", MsgContent = "��J���x����X���~" },
        new MsgModel{ MsgCode = "21114", MsgContent = "�|�L�i�X�����e��" },
        new MsgModel{ MsgCode = "21115", MsgContent = "���e���w��J�x��" },
        new MsgModel{ MsgCode = "21116", MsgContent = "�e�������T�A���i�X��" },
        new MsgModel{ MsgCode = "21117", MsgContent = "���e��{0}�j�w�h�i��{1}�A���i�i��" },
        new MsgModel{ MsgCode = "21118", MsgContent = "���e��{0}�j�w�渹{1}�A�P���f���e���j�w�渹{2}���P�A���i�i��" },
        new MsgModel{ MsgCode = "21119", MsgContent = "���e��{0}�w�j�w���f����{1}�A�P�H����������f��{2}���P�A���i�i��" },
        new MsgModel{ MsgCode = "21120", MsgContent = "���渹{0}�Ĥ@�c�e��{1}���������f�i���T�{�A�Х��N�Ĥ@�c�e���i���T�{��A�i����L�e��" },
        new MsgModel{ MsgCode = "21121", MsgContent = "���e��{0}�j�w���渹{1}�������w�����f��A���i���f�i���T�{�A�Э��s���涰�f�i��" },
        new MsgModel{ MsgCode = "21122", MsgContent = "���Ĥ@�c�e��{0}�j�w�h�i��{1}�A���i�i��" },
        new MsgModel{ MsgCode = "21123", MsgContent = "���e���t�Υ��b�B�z���A�еy��A��" },

        #endregion

        #region 12 �i�f���o
        new MsgModel{ MsgCode = "21201", MsgContent = "�i�f�渹/�f�D�渹���o����" },
        new MsgModel{ MsgCode = "21202", MsgContent = "�L�i�ܳ���" },
        new MsgModel{ MsgCode = "21203", MsgContent = "��֪��i�f�椣�����i�f���o�@�~" },
        new MsgModel{ MsgCode = "21204", MsgContent = "�Ӷi�ܳ�{0}���A��{1}�A�L�k�i�榬�f" },
        new MsgModel{ MsgCode = "21205", MsgContent = "�i�f�渹���o����" },
        new MsgModel{ MsgCode = "21206", MsgContent = "�|���]�w�i�ܧ@�~�P�v��t�ξ�X�Ѽ�" },
        new MsgModel{ MsgCode = "21207", MsgContent = "�ݭn���u�@���s���M�v��t�θj�w�A�д��Ѥu�@���s��" },
        new MsgModel{ MsgCode = "21208", MsgContent = "�u�@���s�� {0} ���]�w�b���f�ϡA������G�}�Y�γq����T�޲z�H���վ�" },
        new MsgModel{ MsgCode = "21209", MsgContent = "�u�@���s�����~�A������J4�X" },
        new MsgModel{ MsgCode = "21210", MsgContent = "�u�@���s�� {0} ���]�w�A�гq����T�޲z�H���s�W�u�@���s��" },
        #endregion

        #region 13 �ӫ~����
        new MsgModel{ MsgCode = "21301", MsgContent = "�ëD�ϥΤ��e��" },
        new MsgModel{ MsgCode = "21302", MsgContent = "�e���w����/��������" },
        new MsgModel{ MsgCode = "21303", MsgContent = "���e���|�����c" },
        new MsgModel{ MsgCode = "21304", MsgContent = "�������}�~�Ǹ��A�ӫ~���i����" },
        new MsgModel{ MsgCode = "21305", MsgContent = "���e�����祢�ѡA�Ц� [���粧�`�B�z�\��]�A�i����򪺧@�~" },
        new MsgModel{ MsgCode = "21306", MsgContent = "�禬�e����ƹ�������Ƥ��o����" },
        new MsgModel{ MsgCode = "21307", MsgContent = "LMS�^��:{0}" },
        new MsgModel{ MsgCode = "21308", MsgContent = "LMS�^�Ъ��~�������T" },
        new MsgModel{ MsgCode = "21309", MsgContent = "�䤣��i�ܳ���" },
        new MsgModel{ MsgCode = "21310", MsgContent = "���X�����T" },
        new MsgModel{ MsgCode = "21311", MsgContent = "���礣�q�L��]���o����" },
        new MsgModel{ MsgCode = "21312", MsgContent = "�s�e�����X���o����" },
        new MsgModel{ MsgCode = "21313", MsgContent = "�s�e���w�Q�ϥΡA�Ч󴫨�L�e��" },
        new MsgModel{ MsgCode = "21314", MsgContent = "�쥻���e���O�V�M���e���A�ä����\�ϥ�" },
        new MsgModel{ MsgCode = "21315", MsgContent = "�e���Y�ɸ�Ʀ��~" },
        new MsgModel{ MsgCode = "21316", MsgContent = "�e�����ɸ�Ʀ��~" },
        new MsgModel{ MsgCode = "21317", MsgContent = "�e���P�禬�����ӫ~�������~" },
        new MsgModel{ MsgCode = "21318", MsgContent = "�®e�������ӫ~���s�b" },
        new MsgModel{ MsgCode = "21319", MsgContent = "�®e�����s�b" },
        new MsgModel{ MsgCode = "21320", MsgContent = "�ФŨϥήƲ���(������)�e��" },
        new MsgModel{ MsgCode = "21321", MsgContent = "���e���w��������A���b���ʤ��A���i���s����" },
        new MsgModel{ MsgCode = "21322", MsgContent = "���e���w��������A�]��F�W�[���u�@���A���i���s����" },
        new MsgModel{ MsgCode = "21323", MsgContent = "�Ӯe���w���c�ݽ���A���o�S���ӫ~���" },
        new MsgModel{ MsgCode = "21324", MsgContent = "�ФŨϥήƲ����e��" },
        new MsgModel{ MsgCode = "21325", MsgContent = "�D�ݤW�[�e���A�нT�{���X" },
        new MsgModel{ MsgCode = "21326", MsgContent = "���e���|�����c" },
        new MsgModel{ MsgCode = "21327", MsgContent = "�ӫ~����������q�L�A�|�����礣�i�W�[" },
        new MsgModel{ MsgCode = "21328", MsgContent = "���e���w���ʨ�W�[���u�@���A���i�A�Q����" },
        new MsgModel{ MsgCode = "21329", MsgContent = "���禬�e��{0}�t�Υ��b�B�z���A�еy��A��" },
        new MsgModel{ MsgCode = "21330", MsgContent = "����q�L�B�z���~" },
        new MsgModel{ MsgCode = "21331", MsgContent = "����q�L�B�z���~" },
        new MsgModel{ MsgCode = "21332", MsgContent = "���e���w����" },
        new MsgModel{ MsgCode = "21333", MsgContent = "���e���w��������" },
        new MsgModel{ MsgCode = "21334", MsgContent = "�d�L�禬����" },
        #endregion

        #region 14 �e���d��
        new MsgModel{ MsgCode = "21401", MsgContent = "�����V�M���e���A�Ȥ����Ѭd��" },
        new MsgModel{ MsgCode = "21402", MsgContent = "�Ӯe���������~" },
        new MsgModel{ MsgCode = "21403", MsgContent = "���e�����X���ݳ�ڸj�w���A�еy��A�d��" },
        new MsgModel{ MsgCode = "21404", MsgContent = "�����S���c�e���A�Ȥ����Ѭd��" },
        
        
        #endregion

        #region 16 �t�h�K�Q��
        new MsgModel{ MsgCode = "21601", MsgContent = "�K�Q�ܽs�����o����" },
        new MsgModel{ MsgCode = "21602", MsgContent = "�t�h�X�f�渹���o����" },
        new MsgModel{ MsgCode = "21603", MsgContent = "�t�h�X�f�渹���s�b" },
        new MsgModel{ MsgCode = "21604", MsgContent = "���t�h�X�f��|���t�w" },
        new MsgModel{ MsgCode = "21605", MsgContent = "���t�h�X�f��|�������]�ˡA���i�J��" },
        new MsgModel{ MsgCode = "21606", MsgContent = "���t�h�X�f��w�X�f�A���i�J��" },
        new MsgModel{ MsgCode = "21607", MsgContent = "���t�h�X�f��w�����A���i�J��" },
        new MsgModel{ MsgCode = "21608", MsgContent = "���t�h�X�f��{0}�A���i�J��" },
        new MsgModel{ MsgCode = "21609", MsgContent = "���t�h�X�f��w�J���t�h�K�Q�ܡA���i�J��" },
        new MsgModel{ MsgCode = "21610", MsgContent = "���x�椣���A�L�k�J��" },
        new MsgModel{ MsgCode = "21611", MsgContent = "�t�ӽs�����o����" },
        new MsgModel{ MsgCode = "21612", MsgContent = "�K�Q���x��s�����o����" },
        new MsgModel{ MsgCode = "21613", MsgContent = "�K�Q���x��s�����s�b" },
        new MsgModel{ MsgCode = "21614", MsgContent = "�K�Q���x��w�Q {0} �t�ӨϥΡA���i��J" },
        new MsgModel{ MsgCode = "21615", MsgContent = "�L���t�� {0} �K�Q�ܤJ�����" },
        new MsgModel{ MsgCode = "21616", MsgContent = "�t�ӽs�����s�b" },
        new MsgModel{ MsgCode = "21617", MsgContent = "�x��s�����o����" },
        new MsgModel{ MsgCode = "21618", MsgContent = "���x��w�L�J����ơA�t�Τw�����x��" },
        new MsgModel{ MsgCode = "21619", MsgContent = "���x��L���t�h�X�f�渹" },
        #endregion

        #region 17 �Ƚc�ɳf
        new MsgModel{ MsgCode = "21701",MsgContent = "�����Ȥw�Q{0}��������"},
        new MsgModel{ MsgCode = "21702",MsgContent = "�d�L����"},
        #endregion

        #region 18 ��w�ռ��禬�J�۰ʭ�
        new MsgModel{ MsgCode = "21801", MsgContent = "�Х��]�w��w�ռ��禬���W�[�ܧO�M��"},
        new MsgModel{ MsgCode = "21802", MsgContent = "�Х��]�w��w�ռ��禬���W�[�ܧO�M��"},
        #endregion 18 ��w�ռ��禬�J�۰ʭ�
      };
    }

    /// <summary>
    /// �b���ˮ�
    /// </summary>
    /// <param name="empID">�b��</param>
    /// <returns></returns>
    public IQueryable<F1924> CheckAcc(string empID)
    {
      var f1924Repository = new F1924Repository(Schemas.CoreSchema);
      return f1924Repository.CheckAcc(empID);
    }

    /// <summary>
    /// �ˮ֤H���\���v��
    /// </summary>
    /// <param name="funcNo">�\��s��</param>
    /// <param name="accNo">�b��</param>
    /// <returns></returns>
    public int CheckAccFunction(string funcNo, string accNo)
    {
      var f192401Repository = new F192401Repository(Schemas.CoreSchema);
      return f192401Repository.CheckAccFunction(funcNo, accNo);
    }

    /// <summary>
    /// �ˮ֤H���f�D�v��
    /// </summary>
    /// <param name="custCode">�f�D�s��</param>
    /// <param name="empId">�b��</param>
    /// <returns></returns>
    public int CheckAccCustCode(string custCode, string empId)
    {
      var f192402Repository = new F192402Repository(Schemas.CoreSchema);
      return f192402Repository.CheckAccCustCode(custCode, empId);
    }

    /// <summary>
    /// �ˮ֤H�����y�����v��
    /// </summary>
    /// <param name="dcCode">���y���߽s��</param>
    /// <param name="empId">�b��</param>
    /// <returns></returns>
    public int CheckAccDc(string dcCode, string empId)
    {
      var f192402Repository = new F192402Repository(Schemas.CoreSchema);
      return f192402Repository.CheckAccDc(dcCode, empId);
    }

    /// <summary>
    /// ���o�~�D�s��
    /// </summary>
    /// <param name="custCode">�f�D�s��</param>
    /// <returns></returns>
    public string GetGupCode(string custCode)
    {
      var f1909Repository = new F1909Repository(Schemas.CoreSchema);
      return f1909Repository.GetGupCode(custCode);
    }

    /// <summary>
    /// �ˮֱb���O�_�w�n�J�b��L�˸m
    /// </summary>
    /// <param name="accNo">�b��</param>
    /// <returns></returns>
    public bool CheckLoginLog(string accNo, string mcCode)
    {
      var f0070Repository = new F0070Repository(Schemas.CoreSchema);
      return f0070Repository.CheckLoginLog(accNo, mcCode);
    }

    /// <summary>
    /// ���o�H���W��
    /// </summary>
    /// <param name="empId">�b��</param>
    /// <returns></returns>
    public string GetEmpName(string empID)
    {
      var f1924Repository = new F1924Repository(Schemas.CoreSchema);
      return f1924Repository.GetEmpName(empID);
    }

    /// <summary>
    /// ���o�ռ����u�s��
    /// </summary>
    /// <param name="detailList"></param>
    /// <returns></returns>
    public List<GetRouteListRes> GetRouteList(List<GetRouteListReq> detailList)
    {
      List<GetRouteListRes> result = new List<GetRouteListRes>();

      detailList = detailList.OrderBy(x => x.LocCode).ToList();

      for (int i = 0; i < detailList.Count; i++)
      {
        var currData = detailList[i];

        result.Add(new GetRouteListRes
        {
          Route = (i + 1),
          No = currData.No,
          Seq = currData.Seq,
          LocCode = currData.LocCode
        });
      }

      return result;
    }

    /// <summary>
    /// ���o�ӫ~�Ǹ��M��
    /// </summary>
    /// <param name="dcCode">���y���߽s��</param>
    /// <param name="custCode">�f�D�s��</param>
    /// <param name="gupCode">�~�D�s��</param>
    /// <param name="itemCode">�ӫ~�s��</param>
    /// <param name="rtNo">�禬�渹</param>
    /// <param name="wmsNo">�t�γ渹</param>
    /// <returns></returns>
    public IQueryable<GetAllocItemSn> GetSnList(string dcCode, string custCode, string gupCode, List<string> itemCode, List<string> rtNo = null, List<string> snList = null)
    {
      var f2501Repository = new F2501Repository(Schemas.CoreSchema);
      var f02020104Repository = new F02020104Repository(Schemas.CoreSchema);
      if (rtNo != null)
      {
        // ���o�i�ܧǸ��M��
        var getSnlistFromF02020104 = f02020104Repository.GetSnList(dcCode, custCode, gupCode, rtNo);
        // ���o�ӫ~�Ǹ��M��
        return f2501Repository.GetSnList(dcCode, gupCode, custCode, getSnlistFromF02020104.ToList());
      }
      else
      {
        return f2501Repository.GetSnList(gupCode, custCode, itemCode, snList);
      }

    }

    /// <summary>
    /// ���o�T�����e
    /// </summary>
    /// <param name="msgCode">�T���N�X</param>
    /// <returns></returns>
    public string GetMsg(string msgCode)
    {
      List<MsgModel> msgData = GetMsgData();
      MsgModel data = msgData.Where(x => x.MsgCode.Equals(msgCode)).SingleOrDefault();
      return data != null ? data.MsgContent : string.Empty;
    }

    /// <summary>
    /// �ˮָ˸m���ҽX
    /// </summary>
    /// <param name="devCode"></param>
    /// <returns></returns>
    public DbKeyEnum? CheckDevCode(string devCode)
    {
      DbKeyEnum? res = null;
      int number;
      if (devCode != null)
      {
        if (!string.IsNullOrWhiteSpace(devCode) && devCode.Length == 10)
        {
          // decode�ȫe3�X
          string firstDecode = devCode.Substring(0, 3);

          // decode�ȫ�7�X
          string lastDecode = devCode.Substring(3, 7);

          // �ˮ֫e3�X�O�_���Ʀr�A�Y�D�ƭȫh�^��null
          // �ˮ֫�7�X�O�_��A-Z,a-z,0-9�A�p���O�h�^��null
          if (int.TryParse(firstDecode, out number) && new Regex(@"^.[A-Za-z0-9]+$").Match(lastDecode).Success)
          {
            // decode�ȫe3�X�N�C�@�Ӧr���z��ASCII�ഫ�ƭȨò֥[���+��7�X�ƭ� MOD 174 ���l�ƭ�
            int key = (Convert.ToInt32(firstDecode) + Encoding.ASCII.GetBytes(lastDecode).Sum(x => x)) % 174;
            //int key = (Encoding.ASCII.GetBytes(lastDecode).Sum(x => x) + Convert.ToInt32(firstDecode)) % 174;

            // ��X�ŦXkey��Enum
            var dbKeyEnums = System.Enum.GetValues(typeof(DbKeyEnum)).Cast<DbKeyEnum>();
            var currKeyEnum = dbKeyEnums.Where(x => (int)x == key);
            res = currKeyEnum.Count() > 0 ? currKeyEnum.FirstOrDefault() : default(DbKeyEnum?);
          }
        }
      }

      return res;
    }

    /// <summary>
    /// �b���K�X�ˮ�
    /// </summary>
    /// <param name="accNo"></param>
    /// <param name="pwd"></param>
    /// <returns></returns>
    public GetValidateUser ValidateUser(string accNo)
    {
      var f1952Repository = new F1952Repository(Schemas.CoreSchema);
      return f1952Repository.ValidateUser(accNo);
    }

    /// �ˮ֬O�_�n�J�̦��v��
    /// </summary>
    /// <param name="empId">�@�~�H���b��</param>
    /// <param name="dcCode">���y���߽s��</param>
    /// <param name="locCode">����x��</param>
    /// <returns></returns>
    public bool CheckActLoc(string empId, string dcCode, string locCode)
    {
      var f192403Repository = new F192403Repository(Schemas.CoreSchema);
      return f192403Repository.CheckActLoc(empId, dcCode, locCode);
    }
    /// <summary>
    /// �ˮ��x��O�_�ᵲ
    /// </summary>
    /// <param name="dcCode">���y���߽s��</param>
    /// <param name="locCode">����x��</param>
    /// <param name="allocType">������O</param>
    /// <returns></returns>
    public bool CheckLocFreeze(string dcCode, string locCode, string allocType)
    {
      var sharedService = new SharedService();
      return sharedService.CheckLocFreeze(dcCode, locCode, allocType).IsSuccessed;
    }

    /// <summary>
    /// �˫h�ӫ~�O�_�V��
    /// </summary>
    /// <returns></returns>
    public bool CheckItemMixBatch(string dcCode, string gupCode, string custCode, string itemCode, string locCode, string validDate)
    {
      var sharedService = new SharedService();
      return sharedService.CheckItemMixBatch(dcCode, gupCode, custCode, itemCode, locCode, validDate);
    }
    /// <summary>
    /// �˫h�ӫ~�O�_�V�~
    /// </summary>
    /// <returns></returns>
    public bool CheckItemMixLoc(string dcCode, string gupCode, string custCode, string itemCode, string locCode)
    {
      var sharedService = new SharedService();
      return sharedService.CheckItemMixLoc(dcCode, gupCode, custCode, itemCode, locCode);
    }

    public void InsertLoginLog(string mcCode, string accNo, string devCode)
    {
      var f0070Repository = new F0070Repository(Schemas.CoreSchema);
      f0070Repository.InsertLoginLog(mcCode, accNo, devCode);
    }
    /// <summary>
    /// ���o�\���v��
    /// </summary>
    /// <param name="funcNo">�\��s��</param>
    /// <returns></returns>
    public string GetFunName(string funcNo)
    {
      var f1954Repository = new F1954Repository(Schemas.CoreSchema);
      return f1954Repository.GetFunName(funcNo).FirstOrDefault();


    }

    /// <summary>
    /// �ˮ֦r��O�_���Ʀr
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool CheckIsNum(string value)
    {
      int n;
      return !string.IsNullOrWhiteSpace(value) ? int.TryParse(value, out n) : false;
    }

    /// <summary>
    /// �R���n�J����
    /// </summary>
    /// <param name="accNo">�b��</param>
    /// <param name="devCode">�˸m���ҽX</param>
    public void DeleteLoginLog(string accNo, string devCode, string mcCode = null)
    {
      F0070Repository f0070Repository = new F0070Repository(Schemas.CoreSchema);
      f0070Repository.DeleteLoginLog(devCode, mcCode, accNo);
    }

    /// <summary>
    /// ���o�ӫ~���W��
    /// </summary>
    /// <param name="unit">�ӫ~���s��</param>
    /// <returns></returns>
    public string GetItemUnit(string unit)
    {
      F91000302Repository f91000302Repository = new F91000302Repository(Schemas.CoreSchema);
      return f91000302Repository.GetItemUnit(unit);
    }

    /// <summary>
    /// ���o���A�W��
    /// </summary>
    /// <param name="topic">�{���s��(��ƪ�)</param>
    /// <param name="subTopic">���ID</param>
    /// <param name="value">�Ѽƭ�</param>
    /// <returns></returns>
    public string GetTopicValueName(string topic, string subTopic, string value)
    {
      F000904Repository f000904Repository = new F000904Repository(Schemas.CoreSchema);
      return f000904Repository.GetTopicValueName(topic, subTopic, value);
    }

    /// <summary>
    /// ���o���A�W��
    /// </summary>
    /// <param name="topic">�{���s��(��ƪ�)</param>
    /// <param name="subTopic">���ID</param>
    /// <param name="value">�Ѽƭ�</param>
    /// <returns></returns>
    public string GetTopicValueNameByVW(string topic, string subTopic, string value)
    {
      F000904Repository f000904Repository = new F000904Repository(Schemas.CoreSchema);
      return f000904Repository.GetTopicValueNameByVW(topic, subTopic, value);
    }

    public string GetWhName(string dcCode, string warehouseId)
    {
      F1980Repository f1980Repository = new F1980Repository(Schemas.CoreSchema);
      return f1980Repository.GetWhName(dcCode, warehouseId);
    }

    /// <summary>
    /// �ӫ~�żh�P�ܧO�żh��Ӫ�
    ///  �ӫ~�ū�              �ܧO�żh
    ///  02(���),03(�N��) =>  02(�C��)
    ///  01(�`��)          =>  01(�`��)
    ///  04(�N��)          =>  03(�N��) 
    /// </summary>
    /// <param name="itemTmpr"></param>
    /// <returns></returns>
    public string GetWareHouseTmprByItemTmpr(string itemTmpr)
    {
      var sharedService = new SharedService();
      return sharedService.GetWareHouseTmprByItemTmpr(itemTmpr);
    }

    /// <summary>
    /// ��s�ӷ��渹���A
    /// </summary>
    /// <param name="sourceType">�ӷ��������</param>
    /// <param name="dcCode">���y����</param>
    /// <param name="gupCode">�~�D</param>
    /// <param name="custCode">�f�D</param>
    /// <param name="wmsNo">�U����ڳ渹(�D�ӷ��渹)</param>
    /// <param name="wmsNoStatus">��ڳ渹���A(�D�ӷ��渹)</param>
    public ExecuteResult UpdateSourceNoStatus(SourceType sourceType, string dcCode, string gupCode, string custCode, string wmsNo, string wmsNoStatus)
    {
      var sharedService = new SharedService(_wmsTransation);
      return sharedService.UpdateSourceNoStatus(sourceType, dcCode, gupCode, custCode, wmsNo, wmsNoStatus);
    }

    /// <summary>
    /// �M�� �w��}�Ǹ����c��/����/�x�ȥd����
    /// </summary>
    /// <param name="dcCode">���y����</param>
    /// <param name="gupCode">�~�D</param>
    /// <param name="custCode">�f�D</param>
    /// <param name="wmsNo">�U����ڳ渹(�D�ӷ��渹)</param>
    /// <param name="type">�U�[:��TD�A�W�[��TU</param>
    public void ClearSerialByBoxOrCaseNo(string dcCode, string gupCode, string custCode, string wmsNo, string type)
    {
      var serialNoService = new SerialNoService();
      serialNoService.ClearSerialByBoxOrCaseNo(dcCode, gupCode, custCode, wmsNo, type);
    }

    /// <summary>
    /// ��J�n�J�̱b���B�m�W
    /// </summary>
    /// <param name="accNo"></param>
    public void SetDefaulfStaff(StaffModel staff)
    {
      if (!string.IsNullOrWhiteSpace(staff.AccNo) && Current.DefaultStaff != staff.AccNo)
      {
        Current.DefaultStaff = staff.AccNo;
        Current.DefaultStaffName = GetEmpName(staff.AccNo);
      }
    }

    /// <summary>
    /// ��s�n�z�f���x��w�ϥήe�n�q
    /// </summary>
    /// <param name="dcCode">���y����</param>
    /// <param name="gupCode">�~�D</param>
    /// <param name="custCode">�f�D</param>
    /// <param name="pickOrdNos">�z�f�渹</param>
    public void UpdatePickOrdNoLocVolumn(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
    {
      var sharedService = new SharedService();
      sharedService.UpdatePickOrdNoLocVolumn(dcCode, gupCode, custCode, pickOrdNos);
    }

    /// <summary>
    /// ���o�ӫ~�W��
    /// </summary>
    /// <param name="gupCode">�~�D�s��</param>
    /// <param name="custCode">�f�D�s��</param>
    /// <param name="itemCode">�~��</param>
    /// <returns></returns>
    public string GetItemName(string gupCode, string custCode, string itemCode)
    {
      F1903Repository f1903Repository = new F1903Repository(Schemas.CoreSchema);
      return f1903Repository.GetItemName(gupCode, custCode, itemCode);
    }

    /// <summary>
    /// �ˮ��x���v��
    /// </summary>
    /// <param name="dcCode">���y���߽s��</param>
    /// <param name="accNo"></param>
    /// <param name="custCode"></param>
    /// <param name="locCode">�f�D�s��</param>
    /// <param name="loctype">loctype</param>
    /// <returns></returns>
    public ApiResult CheckLocCode(string dcCode, string accNo, string custCode, string locCode, string loctype)
    {
      ApiResult apiResult = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = GetMsg("10001") };
      F1912Repository f1912Repo = new F1912Repository(Schemas.CoreSchema);
      F192403Repository f192403Repo = new F192403Repository(Schemas.CoreSchema);
      // �ˮ��x��O�_�s�b
      var f1912 = f1912Repo.CheckLocExist(dcCode);
      if (apiResult.IsSuccessed && !f1912)
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20053", MsgContent = GetMsg("20053") };
      }

      // �ˮ��x��O�_���n�J�̦��v��
      var f192403 = f192403Repo.CheckActLoc(accNo, dcCode, locCode);
      if (apiResult.IsSuccessed && !f192403)
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20055", MsgContent = GetMsg("20055") };
      }

      // �ˮ֨ӷ��x��O�_�ᵲ �Yloctype=1 (�ӷ���)�A�Yloctype=2 (�ت���)
      SharedService sharedService = new SharedService();
      var checkLocFreeze = sharedService.CheckLocFreeze(dcCode, locCode, loctype);
      if (loctype == "1")
      {
        if (apiResult.IsSuccessed && !checkLocFreeze.IsSuccessed)
        {
          apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20062", MsgContent = checkLocFreeze.Message };
        }
      }
      else if (loctype == "2")
      {
        if (apiResult.IsSuccessed && !checkLocFreeze.IsSuccessed)
        {
          apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20062", MsgContent = checkLocFreeze.Message };
        }
      }

      // �ˮ��x��O�_�D���f�D�x��
      var checkCustCodeLoc = f1912Repo.CheckCustCodeLoc(dcCode, locCode);
      if (apiResult.IsSuccessed && (checkCustCodeLoc.CUST_CODE != custCode && checkCustCodeLoc.CUST_CODE != "0"))
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = GetMsg("20056") };
      }
      else
      {
        if (apiResult.IsSuccessed && (checkCustCodeLoc.NOW_CUST_CODE != custCode && checkCustCodeLoc.NOW_CUST_CODE != "0"))
        {
          apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = GetMsg("20056") };
        }
      }

      return apiResult;
    }

    /// <summary>
    /// �ˮ��x�쪺�żh
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="custCode"></param>
    /// <param name="locCode"></param>
    /// <param name="gupCode"></param>
    /// <returns></returns>
    public ApiResult CheckLocTmpr(string dcCode, string itemCode, string custCode, string locCode, string gupCode)
    {
      P81Service p81Service = new P81Service();
      F1903Repository f1903Repo = new F1903Repository(Schemas.CoreSchema);
      F1912Repository f1912Repo = new F1912Repository(Schemas.CoreSchema);
      SharedService sharedService = new SharedService();
      ApiResult apiResult = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };


      // �ˬd�ӫ~�żh
      var getF1903Tmpr = f1903Repo.GetF1903Tmpr(itemCode, custCode, gupCode);
      if (apiResult.IsSuccessed && getF1903Tmpr == null)
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20072", MsgContent = string.Format(GetMsg("20072"), locCode) };
      }

      // �ˬd�x��żh
      var getF1912Tmpr = f1912Repo.GetF1912Tmpr(dcCode, locCode);
      if (apiResult.IsSuccessed && getF1912Tmpr == null)
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20073", MsgContent = string.Format(GetMsg("20073"), itemCode) };
      }

      if (apiResult.IsSuccessed)
      {
        string newTmpr = p81Service.GetWareHouseTmprByItemTmpr(getF1903Tmpr.TmprType);

        // ����x��żh = �ӫ~�żh���x��żh �p�G���ۦP�h�^�� & ���o�T�����e[20071](�x��, �x��żh�W��, �~��, �ӫ~�żh�W��)
        if (!newTmpr.Split(',').Contains(getF1912Tmpr.TmprType))
        {
          //"�x��{0}�żh{1}���ŦX�ӫ~{2}�żh{3}"
          apiResult = new ApiResult
          {
            IsSuccessed = false,
            MsgCode = "20071",
            MsgContent = string.Format(p81Service.GetMsg("20071"),
                  locCode, getF1912Tmpr.TmprTypeName, getF1903Tmpr.TmprTypeName, itemCode)
          };
        }
      }

      return apiResult;
    }

    /// <summary>
    /// �ˬd�ӫ~�V��V�~
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public ApiResult CheckLocHasItem(string dcCode, string gupCode, string custCode, string itemCode, string locCode, string valiDate)
    {
      P81Service p81Service = new P81Service();
      SharedService sharedService = new SharedService();
      ApiResult apiResult = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

      if (!sharedService.CheckItemMixBatch(dcCode, gupCode, custCode, itemCode, locCode, valiDate))
      {
        apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20057", MsgContent = p81Service.GetMsg("20057") };
      }

      if (apiResult.IsSuccessed)
      {
        if (!sharedService.CheckItemMixLoc(dcCode, gupCode, custCode, itemCode, locCode))
        {
          apiResult = new ApiResult { IsSuccessed = false, MsgCode = "20058", MsgContent = p81Service.GetMsg("20058") };
        }
      }

      return apiResult;
    }

    // ���o�ӫ~�~��
    public F1903 GetItemCode(string custCode, string itemCode)
    {
      var f1903Repository = new F1903Repository(Schemas.CoreSchema);
      return f1903Repository.GetItemCode(custCode, itemCode);
    }

    /// <summary>
    /// �ήe�����o�渹
    /// </summary>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    public List<GetWmsNoByContainerCodeRes> GetWmsNoByContainerCode(string containerCode)
    {
      var result = new List<GetWmsNoByContainerCodeRes>();
      var f0701Repo = new F0701Repository(Schemas.CoreSchema);
      var f0701 = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_CODE == containerCode && o.CONTAINER_TYPE == "0").FirstOrDefault();

      //(1) �YF0701.container_type = 0�A�h
      //	�� F070101.F0701_ID = F0701.ID�B
      //	�^�� F070101.WMS_NO�BGUP_CODE�BCUST_CODE
      if (f0701 != null)
      {
        var f070101Repo = new F070101Repository(Schemas.CoreSchema);
        var f070101s = f070101Repo.GetDatasByF0701Ids(new List<long> { f0701.ID });
        if (f070101s.Any())
        {
          result.AddRange(f070101s.Select(x => new GetWmsNoByContainerCodeRes
          {
            GUP_CODE = x.GUP_CODE,
            CUST_CODE = x.CUST_CODE,
            WMS_NO = x.WMS_NO
          }).ToList());
        }
      }

      return result;
    }

    public List<string> GetItemCodeByBarcode(ref bool isSn, string dcCode, string gupCode, string custCode, string barcode)
    {
      var itemService = new ItemService();
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      var f020302Repo = new F020302Repository(Schemas.CoreSchema);

      F2501 f2501 = null;
      var itemCodes = itemService.FindItems(gupCode, custCode, barcode, ref f2501);

      if (itemCodes.Any())
      {
        isSn = f2501 != null;
        return itemCodes;
      }

      var f020302 = f020302Repo.GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.SERIAL_NO == barcode).FirstOrDefault();
      if (f020302 != null)
      {
        isSn = true;
        return new List<string> { f020302.ITEM_CODE };
      }

      return null;
    }
  }
}
