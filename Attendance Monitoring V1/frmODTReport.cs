﻿using AMS.Classes;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMS
{
    public partial class frmODTReport : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        DataTable dtODT = new DataTable();
        XtraReportODT odtReport = new XtraReportODT();

        public frmODTReport()
        {
            InitializeComponent();
        }

        private void barButtonCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void BindAttendanceToReport()
        {
            using (var adapt = new SqlDataAdapter("usp_SSRS_Attendance_Summary_ODT", Utilities.con))
            {
                adapt.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapt.SelectCommand.Parameters.AddWithValue("@from", Convert.ToDateTime(dateFrom.Text).ToShortDateString());
                adapt.SelectCommand.Parameters.AddWithValue("@to", Convert.ToDateTime(dateTo.Text).ToShortDateString());
                adapt.SelectCommand.Parameters.AddWithValue("@No_of_absence", numericAbsence.Value);
                adapt.SelectCommand.Parameters.AddWithValue("@Address", cmbAddress.EditValue);
                adapt.Fill(dtODT);
                odtReport.DataSource = dtODT;
            }
        }

        private void SetHeaders()
        {
            odtReport.xrLabelPeriod.Text = "Period : " + Convert.ToDateTime(dateFrom.Text).ToShortDateString() + " to " + Convert.ToDateTime(dateTo.Text).ToShortDateString();
            odtReport.xrLabelLocation.Text = cmbAddress.Text;
            odtReport.xrLabelUser.Text = "Generated by: " + frmLogin.Username; 
        }

        private void barButtonGenerate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dateFrom.Text))
            {
                Utilities.ErrorMessage("Start Date is Required");
            }
            else if (string.IsNullOrWhiteSpace(dateTo.Text))
            {
                Utilities.ErrorMessage("End Date is Required");
            }
            else if (string.IsNullOrWhiteSpace(numericAbsence.Text) || numericAbsence.Value == 0)
            {
                Utilities.ErrorMessage("Number of absents must not set to zero");
            }
            else
            {
                BindAttendanceToReport();
                if (dtODT.Rows.Count == 0)
                    Utilities.ErrorMessage("No ODT record(s) found");
                else
                {
                    SetHeaders();
                    odtReport.xrTableCellChurchID.DataBindings.Add("Text", dtODT, dtODT.Columns["Church_Id"].ToString());
                    odtReport.xrTableCellName.DataBindings.Add("Text", dtODT, dtODT.Columns["BrethrenName"].ToString());
                    odtReport.xrTableCellNoGathering.DataBindings.Add("Text", dtODT, dtODT.Columns["no_gathering"].ToString());
                    odtReport.xrTableCellNoAbsents.DataBindings.Add("Text", dtODT, dtODT.Columns["total_absences"].ToString());
                    odtReport.xrTableCellAddress.DataBindings.Add("Text", dtODT, dtODT.Columns["Address"].ToString());
                    odtReport.CreateDocument();

                    ReportPrintTool printTool = new ReportPrintTool(odtReport);
                    printTool.ShowPreview();
                    this.Close();
                }
            }
        }

        private void frmAttendanceReport_Load(object sender, EventArgs e)
        {
            dtODT.Clear();
            Utilities.FillLookUpEdit(cmbAddress, "GET_BRETHREN_ADDRESS", "address", "address");
            dateFrom.Text = DateTime.Now.ToShortDateString();
            dateTo.Text = DateTime.Now.ToShortDateString();
        }
    }
}
