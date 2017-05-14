﻿using AMS.Classes;
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
    public partial class frmNewlyBaptized : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmNewlyBaptized()
        {
            InitializeComponent();
        }

        private void ClearFields()
        {
            txtChurchID.Clear();
            txtFirstname.Clear();
            txtMiddlename.Clear();
            txtLastname.Clear();
            txtChurchID.Focus();
        }

        private bool IsValid()
        {
            //if (string.IsNullOrWhiteSpace(txtChurchID.Text))
            //{
            //    Utilities.ErrorMessage("Church ID is Required");
            //    return false;
            //}
            //else 
            if (string.IsNullOrWhiteSpace(txtFirstname.Text))
            {
                Utilities.ErrorMessage("First name is Required");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(txtLastname.Text))
            {
                Utilities.ErrorMessage("Last name is Required");
                return false;
            }
            else
                return true;
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Utilities.SQLConnectionOpen();
            if (IsValid())
            {
                using (var cmd = new SqlCommand("INSERT_BRETHREN", Utilities.con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BrethrenID", Instances.brethrenAddEdit.BrethrenID());
                    cmd.Parameters.AddWithValue("@ChurchID", txtChurchID.Text);
                    cmd.Parameters.AddWithValue("@Firstname", txtFirstname.Text);
                    cmd.Parameters.AddWithValue("@Middlename", txtMiddlename.Text);
                    cmd.Parameters.AddWithValue("@Lastname", txtLastname.Text);
                    cmd.Parameters.AddWithValue("@GroupID", 0);
                    cmd.Parameters.AddWithValue("@DateOfBaptism", DateTime.Now.ToShortDateString());
                    cmd.Parameters.AddWithValue("@DateOfBirth", "");
                    cmd.Parameters.AddWithValue("@Gender", "");
                    cmd.Parameters.AddWithValue("@CivilStatus", "");
                    cmd.Parameters.AddWithValue("@Street", "");
                    cmd.Parameters.AddWithValue("@Brgy", "");
                    cmd.Parameters.AddWithValue("@CityTown", "");
                    cmd.Parameters.AddWithValue("@Province", "");
                    cmd.Parameters.AddWithValue("@Region", "");
                    cmd.Parameters.AddWithValue("@Employment", "");
                    cmd.Parameters.AddWithValue("@PhoneNo", "");
                    cmd.Parameters.AddWithValue("@ContactPerson", "");
                    cmd.Parameters.AddWithValue("@ContactPersonNo", "");
                    cmd.Parameters.AddWithValue("@Committee_Names", "");
                    cmd.Parameters.AddWithValue("@Status", "Active");
                    cmd.Parameters.AddWithValue("@Remarks", "");
                    cmd.Parameters.AddWithValue("@locale_id", 0);
                    cmd.Parameters.AddWithValue("@Baptizer", "");
                    cmd.Parameters.AddWithValue("@is_contact_person_member", false);
                    cmd.ExecuteNonQuery();
                    Utilities.GenerateSystemLog("Quickly Added Brethren, " + txtFirstname.Text + " " + txtLastname.Text, "Newly Baptized Module", 1);
                }

                using (var TimeIn = new SqlCommand("INSERT_TEMP_ATTENDANCE", Utilities.con))
                {
                    TimeIn.CommandType = CommandType.StoredProcedure;
                    TimeIn.Parameters.AddWithValue("@tempGathering_ID", frmAttendanceMonitoring.GatheringID);
                    TimeIn.Parameters.AddWithValue("@tempChurch_ID", txtChurchID.Text);
                    TimeIn.Parameters.AddWithValue("@tempAttendance_Status", frmAttendanceMonitoring.Status);
                    TimeIn.Parameters.AddWithValue("@tempReal_Time", DateTime.Now.ToShortTimeString());
                    TimeIn.Parameters.AddWithValue("@is_interlocale", false);
                    TimeIn.Parameters.AddWithValue("@remarks", "");
                    TimeIn.Parameters.AddWithValue("@temp_brethren_id", Utilities.GetLastCodeFromTable("brethren_id", "Brethren"));
                    TimeIn.ExecuteNonQuery();
                }
                
                frmAttendanceMonitoring.drAttendance = frmAttendanceMonitoring.dtAttendance.NewRow();
                var dr = frmAttendanceMonitoring.drAttendance;
                dr["temp_brethren_id"] = Utilities.GetLastCodeFromTable("brethren_id", "Brethren");
                dr["tempChurch_ID"] = txtChurchID.Text;
                dr["Name"] = txtFirstname.Text + " " + txtLastname.Text;
                dr["tempReal_Time"] = DateTime.Now.ToShortTimeString();
                dr["tempAttendance_Status"] = (frmAttendanceMonitoring.Status == 1 ? "TIMED IN" : "LATE");
                dr["is_interlocale"] = "NO";
                dr["remarks"] = "";
                frmAttendanceMonitoring.dtAttendance.Rows.Add(dr);
                frmAttendanceMonitoring.dtAttendance.AcceptChanges();

                Utilities.SuccessMessage(txtFirstname.Text + " " + txtLastname.Text + " SUCCESSFULLY REGISTERED AND TIMED IN");
                Instances.attendanceMontoring.lblStatus.Text = txtFirstname.Text + " " + txtLastname.Text + " SUCCESSFULLY REGISTERED AND TIMED IN";
                ClearFields();
                this.Hide();
            }
        }

        private void barBtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Hide();
        }
    }
}