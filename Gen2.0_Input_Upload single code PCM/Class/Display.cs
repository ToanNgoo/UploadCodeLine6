using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace Gen2._0_Input_Upload_single_code_PCM
{
    class Display
    {
        public void ShowModel(DataGridView dgv, DataTable dt)
        {
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewTextBoxColumn col_stt = new DataGridViewTextBoxColumn();
            col_stt.DataPropertyName = "STT";
            col_stt.HeaderText = "STT";
            col_stt.Name = "STT";
            col_stt.ReadOnly = true;
            col_stt.Width = 80;
            col_stt.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns.Add(col_stt);

            DataGridViewTextBoxColumn col_maMol = new DataGridViewTextBoxColumn();
            col_maMol.DataPropertyName = "Ma_model";
            col_maMol.HeaderText = "Ma_model";
            col_maMol.Name = "Ma_model";
            col_maMol.ReadOnly = true;
            col_maMol.Width = 160;
            col_maMol.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns.Add(col_maMol);

            DataGridViewTextBoxColumn col_namMol = new DataGridViewTextBoxColumn();
            col_namMol.DataPropertyName = "Ten_model";
            col_namMol.HeaderText = "Ten_model";
            col_namMol.Name = "Ten_model";
            col_namMol.ReadOnly = true;
            col_namMol.Width = 160;
            col_namMol.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns.Add(col_namMol);

            DataGridViewTextBoxColumn col_kty = new DataGridViewTextBoxColumn();
            col_kty.DataPropertyName = "Ky_tu_dinh_danh";
            col_kty.HeaderText = "Ky_tu_dinh_danh";
            col_kty.Name = "Ky_tu_dinh_danh";
            col_kty.ReadOnly = true;
            col_kty.Width = 160;
            col_kty.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns.Add(col_kty);

            DataGridViewTextBoxColumn col_frCd = new DataGridViewTextBoxColumn();
            col_frCd.DataPropertyName = "Format_code";
            col_frCd.HeaderText = "Format_code";
            col_frCd.Name = "Format_code";
            col_frCd.ReadOnly = true;
            col_frCd.Width = 180;
            col_frCd.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns.Add(col_frCd);

            DataGridViewTextBoxColumn col_cdCod = new DataGridViewTextBoxColumn();
            col_cdCod.DataPropertyName = "Chieu_dai_code";
            col_cdCod.HeaderText = "Chieu_dai_code";
            col_cdCod.Name = "Chieu_dai_code";
            col_cdCod.ReadOnly = true;
            col_cdCod.Width = 160;
            col_cdCod.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns.Add(col_cdCod);

            dgv.DataSource = dt.DefaultView;
            dgv.ClearSelection();
        }
    }
}
