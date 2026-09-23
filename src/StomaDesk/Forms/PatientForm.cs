using System;
using System.Drawing;
using System.Windows.Forms;
using StomaDesk.Data;
using StomaDesk.Models;
using StomaDesk.Services;
using StomaDesk.Ui;

namespace StomaDesk.Forms
{
    /// <summary>
    /// Add or edit a patient. The fields are copied into the Patient only when the user presses Salvează
    /// and every check passes, so Renunță leaves the object untouched.
    /// </summary>
    public partial class PatientForm : Form
    {
        private readonly ClinicStore _store;
        private readonly Patient _patient;
        private Control _firstError;

        /// <summary>Used by the Visual Studio designer only.</summary>
        public PatientForm()
        {
            InitializeComponent();
            Theme.Primary(btnOk);
            Theme.Apply(this);
        }

        public PatientForm(ClinicStore store, Patient patient)
            : this()
        {
            _store = store;
            _patient = patient;
            Text = patient.Id == 0 ? "Pacient nou" : "Date pacient: " + patient.FullName;
            LoadFields();
        }

        /// <summary>Shows the dialog and saves the patient if the user confirms. Returns true when saved.</summary>
        public static bool Edit(IWin32Window owner, ClinicStore store, Patient patient)
        {
            using (var form = new PatientForm(store, patient))
            {
                if (form.ShowDialog(owner) != DialogResult.OK)
                    return false;
            }
            store.SavePatient(patient);
            return true;
        }

        private void LoadFields()
        {
            txtLastName.Text = _patient.LastName;
            txtFirstName.Text = _patient.FirstName;
            txtCnp.Text = _patient.Cnp;
            txtPhone.Text = _patient.Phone;
            txtEmail.Text = _patient.Email;
            txtAddress.Text = _patient.Address;
            txtAllergies.Text = _patient.Allergies;
            txtNotes.Text = _patient.Notes;

            if (_patient.BirthDate.HasValue)
            {
                dtpBirth.Value = _patient.BirthDate.Value;
                dtpBirth.Checked = true;
            }
            else
            {
                dtpBirth.Checked = false;
            }
            ShowCnpInfo();
        }

        private void SaveFields()
        {
            _patient.LastName = txtLastName.Text.Trim();
            _patient.FirstName = txtFirstName.Text.Trim();
            _patient.Cnp = txtCnp.Text.Trim();
            _patient.Phone = txtPhone.Text.Trim();
            _patient.Email = txtEmail.Text.Trim();
            _patient.Address = txtAddress.Text.Trim();
            _patient.Allergies = txtAllergies.Text.Trim();
            _patient.Notes = txtNotes.Text.Trim();
            _patient.BirthDate = dtpBirth.Checked ? (DateTime?)dtpBirth.Value.Date : null;
        }

        private void txtCnp_TextChanged(object sender, EventArgs e)
        {
            ShowCnpInfo();
        }

        /// <summary>Checks the CNP while the user types and fills in the birth date once it is valid.</summary>
        private void ShowCnpInfo()
        {
            string cnp = txtCnp.Text.Trim();
            lblCnpInfo.ForeColor = Color.DimGray;

            if (cnp.Length == 0)
            {
                lblCnpInfo.Text = "Opțional. Din CNP se completează singură data nașterii.";
                return;
            }
            if (cnp.Length < 13)
            {
                lblCnpInfo.Text = string.Format("{0} din 13 cifre", cnp.Length);
                return;
            }

            CnpInfo info;
            string error;
            if (!Cnp.TryParse(cnp, out info, out error))
            {
                lblCnpInfo.ForeColor = Color.Firebrick;
                lblCnpInfo.Text = error;
                return;
            }

            string who = info.Male == true ? "Bărbat, născut" : info.Male == false ? "Femeie, născută" : "Cetățean străin, născut";
            lblCnpInfo.Text = string.Format("{0} la {1}", who, Fmt.Date(info.BirthDate));
            dtpBirth.Value = info.BirthDate;
            dtpBirth.Checked = true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            _firstError = null;

            Require(txtLastName, "Numele este obligatoriu.");
            Require(txtFirstName, "Prenumele este obligatoriu.");

            string cnp = txtCnp.Text.Trim();
            if (cnp.Length > 0)
            {
                CnpInfo info;
                string error;
                if (!Cnp.TryParse(cnp, out info, out error))
                    Fail(txtCnp, error);
                else if (_store.IsCnpTaken(cnp, _patient.Id))
                    Fail(txtCnp, "Există deja un pacient cu acest CNP.");
            }

            if (txtPhone.Text.Trim().Length > 0 && !PhoneNumber.IsValid(txtPhone.Text))
                Fail(txtPhone, "Numărul de telefon are 10 cifre, de exemplu 0722 123 456.");

            string email = txtEmail.Text.Trim();
            int at = email.IndexOf('@');
            if (email.Length > 0 && (at < 1 || email.LastIndexOf('.') < at))
                Fail(txtEmail, "Adresa de email nu pare validă.");

            if (dtpBirth.Checked && dtpBirth.Value.Date > DateTime.Today)
                Fail(dtpBirth, "Data nașterii este în viitor.");

            if (_firstError != null)
            {
                _firstError.Focus();
                return;
            }

            SaveFields();
            DialogResult = DialogResult.OK;
        }

        private void Require(Control control, string message)
        {
            if (control.Text.Trim().Length == 0)
                Fail(control, message);
        }

        private void Fail(Control control, string message)
        {
            errorProvider.SetError(control, message);
            if (_firstError == null)
                _firstError = control;
        }
    }
}
