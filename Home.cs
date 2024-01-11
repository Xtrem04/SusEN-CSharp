﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SusEN
{
    public partial class Home : Form
    {
        #region Global Variables
        private readonly Font Normal = new Font("Arial", 12.0f, FontStyle.Regular);
        private readonly Font Special = new Font("Microsoft Sans Serif", 12.0f, FontStyle.Bold);
        private readonly string Accounts = "Accounts.txt";
        private readonly string GameInfo = "Game Info.txt";
        private readonly string Recipes = "Recipes.txt";
        private readonly string SystemMessage = "// Generated by SusEN";
        private readonly string Username = "Enter your Username";
        private readonly string Password = "Enter your Password";
        private readonly int StartingCoins = 500;
        private int Level;
        private bool PasswordReady;
        private bool AccountReady;
        #endregion

        #region Constructor
        public Home()
        {
            InitializeComponent();
            if (!File.Exists(Accounts))
            {
                File.WriteAllText(Accounts, $"{SystemMessage}{Environment.NewLine}");
            }
            if (!File.Exists(GameInfo))
            {
                File.WriteAllText(GameInfo, $"{SystemMessage}{Environment.NewLine}");
            }
            if (!File.Exists(Recipes))
            {
                File.WriteAllText(Recipes, $"{SystemMessage}{Environment.NewLine}");
            }
            Default();
        }
        #endregion

        #region Login TextBoxes

        #region Username TextBox
        private void Username_TextBox_Enter(object sender, EventArgs e)
        {
            Username_Error.SetError(Username_TextBox, null);
            if (Username_TextBox.Text == Username)
            {
                Username_TextBox.Text = String.Empty;
                Username_TextBox.ForeColor = Color.Black;
                Username_TextBox.Font = Normal;
            }
        }
        private void Username_TextBox_Leave(object sender, EventArgs e)
        {
            string[] AllAccounts = File.ReadAllLines(Accounts);
            IEnumerable<string> AccountFound = AllAccounts.Where(x => x.Split('|')[0] == Username_TextBox.Text);
            if (Username_TextBox.Text == String.Empty)
            {
                Username_TextBox.Text = Username;
                Username_TextBox.ForeColor = Color.Gray;
                Username_TextBox.Font = Special;
            }
            if (AccountFound.Any())
            {
                Username_TextBox.ForeColor = Color.Green;
            }
        }
        private void Username_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back;
            Username_TextBox.ForeColor = Color.Black;
            Password_PictureBox.Image = Properties.Resources.Closed_Locker;
            if (PasswordReady)
            {
                Password_TextBox.ForeColor = Color.Black;
            }
        }
        private void Username_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string[] AllAccounts = File.ReadAllLines(Accounts);
            IEnumerable<string> AccountUsernameFound = AllAccounts.Where(x => x.Split('|')[0] == Username_TextBox.Text);
            IEnumerable<string> AccountPasswordFound = AccountUsernameFound.Where(x => x.Split('|')[1] == Encrypter.EncryptString(Password_TextBox.Text).Replace('|', '_').Replace('\n', '_').Replace('\r', '_'));
            if (AccountUsernameFound.Any())
            {
                Username_TextBox.ForeColor = Color.Green;
            }
            if (AccountPasswordFound.Any())
            {
                Password_TextBox.ForeColor = Color.Green;
                Password_PictureBox.Image = Properties.Resources.Opened_Locker;
            }
        }
        #endregion

        #region Password TextBox
        private void Password_TextBox_Enter(object sender, EventArgs e)
        {
            Password_TextBox.PasswordChar = !Password_CheckBox.Checked ? (char)0x25CF : (char)0;
            Password_Error.SetError(Password_TextBox, null);
            if (Password_TextBox.Text == Password)
            {
                Password_TextBox.Text = String.Empty;
                Password_TextBox.ForeColor = Color.Black;
                Password_TextBox.Font = Normal;
                PasswordReady = true;
            }
        }
        private void Password_TextBox_Leave(object sender, EventArgs e)
        {
            string[] AllAccounts = File.ReadAllLines(Accounts);
            IEnumerable<string> AccountUsernameFound = AllAccounts.Where(x => x.Split('|')[0] == Username_TextBox.Text);
            IEnumerable<string> AccountPasswordFound = AccountUsernameFound.Where(x => x.Split('|')[1] == Encrypter.EncryptString(Password_TextBox.Text).Replace('|', '_').Replace('\n', '_').Replace('\r', '_'));
            if (Password_TextBox.Text == String.Empty)
            {
                Password_TextBox.Text = Password;
                Password_TextBox.ForeColor = Color.Gray;
                Password_TextBox.Font = Special;
                PasswordReady = false;
                Password_TextBox.PasswordChar = (char)0;
            }
            if (AccountPasswordFound.Any())
            {
                Password_TextBox.ForeColor = Color.Green;
                Password_PictureBox.Image = Properties.Resources.Opened_Locker;
            }
        }
        private void Password_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == (char)Keys.Space;
            Password_TextBox.ForeColor = Color.Black;
            Password_PictureBox.Image = Properties.Resources.Closed_Locker;
        }
        private void Password_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            string[] AllAccounts = File.ReadAllLines(Accounts);
            IEnumerable<string> AccountUsernameFound = AllAccounts.Where(x => x.Split('|')[0] == Username_TextBox.Text);
            IEnumerable<string> AccountPasswordFound = AccountUsernameFound.Where(x => x.Split('|')[1] == Encrypter.EncryptString(Password_TextBox.Text).Replace('|', '_').Replace('\n', '_').Replace('\r', '_'));
            if (AccountPasswordFound.Any())
            {
                Password_TextBox.ForeColor = Color.Green;
                Password_PictureBox.Image = Properties.Resources.Opened_Locker;
            }
        }
        private void Password_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Password_TextBox.PasswordChar = PasswordReady && !Password_CheckBox.Checked ? (char)0x25CF : (char)0;
        }
        #endregion

        #endregion

        #region Login
        private void Login_Button_Click(object sender, EventArgs e)
        {
            string[] AllAccounts = File.ReadAllLines(Accounts);
            string[] AllGameInfos = File.ReadAllLines(GameInfo);
            string[] AllRecipes = File.ReadAllLines(Recipes);
            IEnumerable<string> AccountUsernameFound = AllAccounts.Where(x => x.Split('|')[0] == Username_TextBox.Text);
            IEnumerable<string> AccountPasswordFound = AccountUsernameFound.Where(x => x.Split('|')[1] != Encrypter.EncryptString(Password_TextBox.Text).Replace('|', '_').Replace('\n', '_').Replace('\r', '_'));
            if (Username_TextBox.Text == String.Empty || Username_TextBox.Text == Username)
            {
                Username_Error.SetError(Username_TextBox, "Username cannot be blank");
                return;
            }
            else if (!AccountUsernameFound.Any())
            {
                Username_Error.SetError(Username_TextBox, "Username not found");
                return;
            }
            if (Password_TextBox.Text == String.Empty || Password_TextBox.Text == Password)
            {
                Password_Error.SetError(Password_TextBox, "Password cannot be blank");
            }
            else if (AccountPasswordFound.Any())
            {
                Password_Error.SetError(Password_TextBox, "Password incorrect");
            }
            else
            {
                string UserAccountFound = AllAccounts.Single(x => x.Split('|')[0] == Username_TextBox.Text);
                string[] UserAccount = UserAccountFound.Split('|');
                string UserGameInfoFound = AllGameInfos.Single(x => x.Split('|')[0] == Username_TextBox.Text);
                string[] UserGameInfo = UserGameInfoFound.Split('|');
                string UserRecipesFound = AllRecipes.Single(x => x.Split('|')[0] == Username_TextBox.Text);
                string[] UserRecipes = UserRecipesFound.Split('|');
                SusEN Form = new SusEN(UserAccount, UserGameInfo, UserRecipes);
                Form.Show();
                Hide();
            }
        }
        private void CreateAccount_Label_Click(object sender, EventArgs e)
        {
            TabControl.SelectTab(1);
        }
        #endregion

        #region Create Account TextBoxes

        #region Create Account Username TextBox
        private void CreateAccount_Username_TextBox_Enter(object sender, EventArgs e)
        {
            CreateAccount_Username_Error.SetError(CreateAccount_Username_TextBox, null);
        }
        private void CreateAccount_Username_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back;
        }
        #endregion

        #region Create Account Password TextBox

        #region PasswordMeter
        private void CreateAccount_Password_TextBox_TextChanged(object sender, EventArgs e)
        {
            Default();
            PasswordMeter.Value = PasswordMeter.Minimum;
            Level = 0;
            if (CreateAccount_Password_TextBox.Text.Length >= 8)
            {
                PasswordMeter.Value += PasswordMeter.Step;
                Level += 4;
                MinCharacters_PictureBox.Image = Properties.Resources.Checked;
                MinCharacters_Label.ForeColor = Color.LimeGreen;
            }
            if (CreateAccount_Password_TextBox.Text.Any(char.IsLower))
            {
                PasswordMeter.Value += PasswordMeter.Step / 2;
                Level++;
                LowerCaseLetter_PictureBox.Image = Properties.Resources.Checked;
                LowerCaseLetter_Label.ForeColor = Color.LimeGreen;
            }
            if (CreateAccount_Password_TextBox.Text.Any(char.IsUpper))
            {
                PasswordMeter.Value += PasswordMeter.Step / 2;
                Level++;
                UpperCaseLetter_PictureBox.Image = Properties.Resources.Checked;
                UpperCaseLetter_Label.ForeColor = Color.LimeGreen;
            }
            if (CreateAccount_Password_TextBox.Text.Any(char.IsDigit))
            {
                PasswordMeter.Value += PasswordMeter.Step;
                Level += 4;
                Number_PictureBox.Image = Properties.Resources.Checked;
                Number_Label.ForeColor = Color.LimeGreen;
            }
            if (CreateAccount_Password_TextBox.Text.Any(char.IsSymbol) || CreateAccount_Password_TextBox.Text.Any(char.IsPunctuation))
            {
                PasswordMeter.Value += PasswordMeter.Step;
                Level += 4;
                Symbol_PictureBox.Image = Properties.Resources.Checked;
                Symbol_Label.ForeColor = Color.LimeGreen;
            }
            if (CreateAccount_Password_TextBox.Text.Length >= 12)
            {
                PasswordMeter.Value += PasswordMeter.Step;
                Level += 4;
                MaxCharacters_PictureBox.Image = Properties.Resources.Checked;
                MaxCharacters_Label.ForeColor = Color.LimeGreen;
            }
            for (int i = 0; i <= PasswordMeter.Maximum / 5; i++)
            {
                if (i > 0)
                {
                    Color[] ProgressColor = new Color[] { Color.DarkRed, Color.Red, Color.Orange, Color.Yellow, Color.LightGreen, Color.Green };
                    if (PasswordMeter.Value == (PasswordMeter.Step * i) - 10)
                    {
                        PasswordMeter.ForeColor = ProgressColor[i - 1];
                    }
                    if (PasswordMeter.Value == PasswordMeter.Step * i)
                    {
                        PasswordMeter.ForeColor = Level == 4 * i ? ProgressColor[i - 1] : ProgressColor[i];
                    }
                }
            }
        }

        #region Default
        private void Default()
        {
            MinCharacters_PictureBox.Image = Properties.Resources.UnChecked;
            MinCharacters_Label.ForeColor = Color.Red;
            LowerCaseLetter_PictureBox.Image = Properties.Resources.UnChecked;
            LowerCaseLetter_Label.ForeColor = Color.Red;
            UpperCaseLetter_PictureBox.Image = Properties.Resources.UnChecked;
            UpperCaseLetter_Label.ForeColor = Color.Red;
            Number_PictureBox.Image = Properties.Resources.UnChecked;
            Number_Label.ForeColor = Color.Red;
            Symbol_PictureBox.Image = Properties.Resources.UnChecked;
            Symbol_Label.ForeColor = Color.Red;
            MaxCharacters_PictureBox.Image = Properties.Resources.UnChecked;
            MaxCharacters_Label.ForeColor = Color.Red;
        }
        #endregion

        #endregion

        private void CreateAccount_Password_TextBox_Enter(object sender, EventArgs e)
        {
            CreateAccount_Password_Error.SetError(CreateAccount_Password_TextBox, null);
        }
        private void CreateAccount_Password_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == (char)Keys.Space;
        }
        #endregion

        #region Create Account Confirm Password TextBox
        private void CreateAccount_ConfirmPassword_TextBox_Enter(object sender, EventArgs e)
        {
            CreateAccount_ConfirmPassword_Error.SetError(CreateAccount_ConfirmPassword_TextBox, null);
        }
        private void CreateAccount_ConfirmPassword_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == (char)Keys.Space;
        }
        #endregion

        #endregion

        #region Create Account
        private void CreateAccount_Button_Click(object sender, EventArgs e)
        {
            AccountReady = true;
            string[] AllAccounts = File.ReadAllLines(Accounts);
            IEnumerable<string> AccountFound = AllAccounts.Where(x => x.Split('|')[0] == CreateAccount_Username_TextBox.Text);
            if (CreateAccount_Username_TextBox.Text.Length < 5)
            {
                CreateAccount_Username_Error.SetError(CreateAccount_Username_TextBox, "Username must contain more than 5 characters");
                AccountReady = false;
            }
            else if (AccountFound.Any())
            {
                CreateAccount_Username_Error.SetError(CreateAccount_Username_TextBox, "Username already exists");
                AccountReady = false;
            }
            if (CreateAccount_Password_TextBox.Text.Length < 8)
            {
                CreateAccount_Password_Error.SetError(CreateAccount_Password_TextBox, "Password must contain more than 8 characters");
                AccountReady = false;
            }
            else if (!CreateAccount_Password_TextBox.Text.Any(char.IsLower))
            {
                CreateAccount_Password_Error.SetError(CreateAccount_Password_TextBox, "Password must contain a lower case letter");
                AccountReady = false;
            }
            else if (!CreateAccount_Password_TextBox.Text.Any(char.IsUpper))
            {
                CreateAccount_Password_Error.SetError(CreateAccount_Password_TextBox, "Password must contain an upper case letter");
                AccountReady = false;
            }
            else if (CreateAccount_Password_TextBox.Text.All(char.IsLetter))
            {
                CreateAccount_Password_Error.SetError(CreateAccount_Password_TextBox, "Password must contain a digit or symbol");
                AccountReady = false;
            }
            if (CreateAccount_Password_TextBox.Text != CreateAccount_ConfirmPassword_TextBox.Text)
            {
                CreateAccount_ConfirmPassword_Error.SetError(CreateAccount_ConfirmPassword_TextBox, "Does not match password");
                AccountReady = false;
            }
            if (AccountReady)
            {
                File.AppendAllText(Accounts, $"{CreateAccount_Username_TextBox.Text}|{Encrypter.EncryptString(CreateAccount_Password_TextBox.Text).Replace('|', '_').Replace('\n', '_').Replace('\r', '_')}|User{Environment.NewLine}");
                File.AppendAllText(GameInfo, $"{CreateAccount_Username_TextBox.Text}|{StartingCoins}|{0}{Environment.NewLine}");
                File.AppendAllText(Recipes, $"{CreateAccount_Username_TextBox.Text}|{0}|{0}|{0}|{0}|{0}|{0}|{0}|{0}|{0}|{0}|{0}|{0}|{0}|{0}|{0}{Environment.NewLine}");
                DialogResult Message = MessageBox.Show("Account Created Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (Message == DialogResult.OK)
                {
                    TabControl.SelectTab(0);
                }
            }
        }
        #endregion
    }
}