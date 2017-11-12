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
    public partial class frmAttendanceReport : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        DataTable dtAttendance = new DataTable();
        XtraReportAttendance attendanceReport = new XtraReportAttendance();

        public frmAttendanceReport()
        {
            InitializeComponent();
        }

        private void barButtonCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void BindAttendanceToReport()
        {
            using (var adapt = new SqlDataAdapter("ATTENDANCE_REPORT_NEW", Utilities.con))
            {
                adapt.SelectCommand.CommandType = CommandType.StoredProcedure;
                adapt.SelectCommand.Parameters.AddWithValue("@Gathering_Type", cmbGatheringType.GetColumnValue("gathering_code").ToString());
                adapt.SelectCommand.Parameters.AddWithValue("@Date", Convert.ToDateTime(dtDateGathering.Text).ToShortDateString());
                adapt.Fill(dtAttendance);
                attendanceReport.DataSource = dtAttendance;
            }
        }

        private void SetHeaders()
        {
            attendanceReport.xrLabelGathering.Text = cmbGatheringType.Text;
            attendanceReport.xrLabelDate.Text = Convert.ToDateTime(dtDateGathering.Text).ToShortDateString();
            attendanceReport.xrLabelUser.Text = "Generated by: " + frmLogin.Username;
        }

        private void barButtonGenerate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbGatheringType.Text))
            {
                Utilities.ErrorMessage("Please Select Gathering Type");
            }
            else if (string.IsNullOrWhiteSpace(dtDateGathering.Text))
            {
                Utilities.ErrorMessage("Please Enter Date of Gathering");
            }
            else
            {
                BindAttendanceToReport();

                if (dtAttendance.Rows.Count == 0)
                    Utilities.ErrorMessage("Gathering/Batches does not exists.");
                else
                { 
                    SetHeaders();

                    GroupField gfBatch = new GroupField(dtAttendance.Columns["batch_time"].ToString());
                    GroupField gfInterlocale = new GroupField(dtAttendance.Columns["Interlocale_Status"].ToString());

                    attendanceReport.GroupHeader.GroupFields.Add(gfInterlocale);
                    attendanceReport.GroupHeader.GroupFields.Add(gfBatch);

                    attendanceReport.xrLabelAddPro.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["addpro"].ToString());
                    attendanceReport.xrLabelInterlocale.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["Interlocale_Status"].ToString());
                    attendanceReport.xrLabelBatch.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["batch_time_lbl"].ToString());
                    attendanceReport.xrTableCellChurchID.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["Church_Id"].ToString());
                    attendanceReport.xrTableCellName.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["BrethrenName"].ToString());
                    attendanceReport.xrTableCellTimeIn.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["time_in"].ToString());
                    attendanceReport.xrTableCellStatus.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["AttendanceStatus"].ToString());
                    attendanceReport.xrTableCellLocale.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["locale"].ToString());
                    //attendanceReport.xrLabelAddPro.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["addpro"].ToString());
                    //attendanceReport.xrLabelOfficers.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["officers"].ToString());
                    //attendanceReport.xrLabelWorkers.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["workers"].ToString());
                    //attendanceReport.xrLabelRemarks.DataBindings.Add("Text", dtAttendance, dtAttendance.Columns["remarks"].ToString());
                    attendanceReport.Name = cmbGatheringType.Text.ToString().Replace(" ", string.Empty);

                    attendanceReport.CreateDocument();
                    ReportPrintTool printTool = new ReportPrintTool(attendanceReport);
                    printTool.ShowPreviewDialog();
                    this.Close();
                }
            }
        }

        private void frmAttendanceReport_Load(object sender, EventArgs e)
        {
            dtAttendance.Clear();
            Utilities.FillLookUpEdit(cmbGatheringType, "GET_GATHERING_TYPES", "gathering", "gathering_id");
            dtDateGathering.Text = DateTime.Now.ToShortDateString();
        }

        private void btnViewGatheringList_Click(object sender, EventArgs e)
        {
            frmGatheringSelection gatheringSelection = new frmGatheringSelection();
            gatheringSelection.ShowDialog();
            this.Hide();
        }
    }
}
