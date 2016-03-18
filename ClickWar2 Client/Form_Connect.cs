using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClickWar2.Game.Network;
using ClickWar2.Utility;

namespace ClickWar2_Client
{
    public partial class Form_Connect : Form
    {
        public Form_Connect()
        {
            InitializeComponent();


#if DEBUG
            this.textBox_address.Text = "127.0.0.1";
            this.textBox_port.Text = "3344";
            this.textBox_name.Text = "뭐지What";
            this.textBox_password.Text = "abcd1234";
#else
            this.textBox_address.Text = "$OfficialServer";
#endif
        }

        //#####################################################################################

        protected GameClient m_client = null;

        //#####################################################################################

        private void Form_Connect_Load(object sender, EventArgs e)
        {
#if DEBUG
            this.checkBox_official.Checked = false;
#endif


            this.checkBox_autoLogin.Checked = RegistryHelper.GetDataAsBool("AutoLoginFlag", false);

            if (this.checkBox_autoLogin.Checked)
            {
                // 레지스트리에서 로그인 정보 가져옴
                this.textBox_name.Text = RegistryHelper.GetData("LoginName", "");

                try
                {
                    string key = RegistryHelper.GetData("LoginKey", "");
                    this.textBox_password.Text = Security.DecodeEx(RegistryHelper.GetData("LoginPass", ""),
                        key);

                    // 로그인 시도
                    BeginLogin();
                }
                catch (Exception)
                {
                    MessageBox.Show("자동로그인 정보가 올바르지 않습니다.\n수동로그인을 시도하세요.",
                        "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form_Connect_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.timer_update.Stop();

            if (m_client != null)
            {
                m_client.Disconnect();
                m_client = null;
            }

            Application.Exit();
        }

        private void checkBox_official_CheckedChanged(object sender, EventArgs e)
        {
            bool bChecked = this.checkBox_official.Checked;

            this.textBox_address.Enabled = !bChecked;

            if (bChecked)
            {
                this.textBox_address.Text = "$OfficialServer";
                this.textBox_port.Text = "";
            }
            else
            {
                this.textBox_address.Focus();
                this.textBox_address.SelectAll();
            }
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            BeginLogin();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            if (m_client != null)
            {
                m_client.Disconnect();
                m_client = null;
            }

            Application.Exit();
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            if (m_client != null)
            {
                m_client.Update();
            }
        }

        //#####################################################################################

        protected void BeginLogin()
        {
            DisableUI();


            this.textBox_name.Text = this.textBox_name.Text.Trim();


            if (this.textBox_name.TextLength > 0
                && this.textBox_password.TextLength > 0)
            {
                string address = "", port = "";

                if (this.checkBox_official.Checked
                    || this.textBox_address.TextLength <= 0
                    || this.textBox_address.Text == "$OfficialServer")
                {
                    LoadOfficialServer(out address, out port);

                    // 공식서버로 연결되는 것이라고 해도 포트 번호가 적혀있으면 해당 포트로 연결한다.
                    if (this.textBox_port.TextLength > 0)
                    {
                        int tempPort;
                        if (int.TryParse(this.textBox_port.Text, out tempPort))
                            port = this.textBox_port.Text;
                    }
                }
                else if (this.textBox_address.TextLength > 0
                    && this.textBox_port.TextLength >= 4)
                {
                    address = this.textBox_address.Text;

                    int tempPort;
                    if (int.TryParse(this.textBox_port.Text, out tempPort))
                        port = this.textBox_port.Text;
                }


                if (address.Length > 0 && port.Length > 0)
                {
                    // 클라이언트 생성 및 접속
                    if (m_client == null)
                    {
                        m_client = new GameClient();

                        m_client.Connect(address, port);
                    }


                    if (m_client.IsConnected)
                    {
                        // 자동 로그인 정보 저장
                        this.SaveAutoLoginInformation();


                        // 로그인 요청
                        m_client.SignDirector.Login(this.textBox_name.Text, this.textBox_password.Text,
                            this.WhenReceiveLoginResult);

                        this.timer_update.Start();


                        // UI 활성화하지 않게 넘어감
                        return;
                    }
                    else
                    {
                        m_client.Disconnect();
                        m_client = null;

                        MessageBox.Show("서버에 접속할 수 없습니다.", "Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("접속정보가 유효하지 않습니다.", "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.textBox_port.Select();
                }
            }
            else
            {
                MessageBox.Show("포트가 4자리 이상이 아니거나\n빈칸이 다 채워지지 않았습니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            EnableUI();
        }

        protected void SaveAutoLoginInformation()
        {
            if (this.checkBox_autoLogin.Checked)
            {
                RegistryHelper.SetData("LoginName", this.textBox_name.Text);

                string key;
                string pass = Security.EncodeEx(this.textBox_password.Text, out key);
                RegistryHelper.SetData("LoginPass", pass);

                RegistryHelper.SetData("LoginKey", key);
            }
            else
            {
                RegistryHelper.SetData("LoginName", "");
                RegistryHelper.SetData("LoginPass", "");
                RegistryHelper.SetData("LoginKey", "");
            }

            RegistryHelper.SetData<bool>("AutoLoginFlag", this.checkBox_autoLogin.Checked);
        }

        protected bool LoadOfficialServer(out string address, out string port)
        {
            return ClickWar2.Application.GetOfficialServer("OfficialServer", out address, out port);
        }

        protected void EnableUI()
        {
            if (this.checkBox_official.Checked == false)
            {
                this.textBox_address.Enabled = true;
            }
            this.textBox_port.Enabled = true;

            this.checkBox_official.Enabled = true;

            this.textBox_name.Enabled = true;
            this.textBox_password.Enabled = true;
            this.checkBox_autoLogin.Enabled = true;
            this.button_connect.Enabled = true;
        }

        protected void DisableUI()
        {
            this.textBox_address.Enabled = false;
            this.textBox_port.Enabled = false;

            this.checkBox_official.Enabled = false;

            this.textBox_name.Enabled = false;
            this.textBox_password.Enabled = false;
            this.checkBox_autoLogin.Enabled = false;
            this.button_connect.Enabled = false;
        }

        //#####################################################################################

        protected void SequenceToNext()
        {
            this.Hide();

            foreach (Form form in Application.OpenForms)
            {
                if (form is Form_Main)
                {
                    ((Form_Main)form).Reset(m_client);
                    m_client = null;
                    form.Show();
                    return;
                }
            }

            var nextForm = new Form_Main(m_client);
            m_client = null;
            nextForm.Show();
        }

        private void WhenReceiveLoginResult(LoginResults result)
        {
            this.timer_update.Stop();


            if (result == LoginResults.Success)
            {
                SequenceToNext();
            }
            else if (result == LoginResults.Fail_AlreadyLogin)
            {
                MessageBox.Show("이미 로그인 상태인 계정입니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result == LoginResults.Fail_NotUser)
            {
                var selection = MessageBox.Show("존재하지않는 계정입니다.\n현재 기입된 정보로 등록하시겠습니까?", "Error!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (selection == DialogResult.Yes)
                {
                    if (this.colorDialog_userColor.ShowDialog() == DialogResult.OK)
                    {
                        Color color = this.colorDialog_userColor.Color;

                        if (color.R >= 100 || color.G >= 100 || color.B >= 100)
                        {
                            // 회원가입 요청
                            m_client.SignDirector.Register(this.textBox_name.Text, this.textBox_password.Text,
                                color,
                                this.WhenReceiveRegisterResult);

                            this.timer_update.Start();


                            // UI 활성화 안되게 함.
                            return;
                        }
                        else
                        {
                            MessageBox.Show("색이 너무 어둡습니다.\nRGB 값 중 하나라도 100 이상이여야 합니다.", "Error!",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("색을 선택해주셔야 합니다.", "Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (result == LoginResults.Fail_WrongPassword)
            {
                MessageBox.Show("비밀번호가 틀립니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result == LoginResults.Fail_DifferentVersion)
            {
                MessageBox.Show("클라이언트의 버전이 서버와 다릅니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result == LoginResults.Fail_ServerNotReady)
            {
                MessageBox.Show("아직 서버가 준비 중 입니다.\n잠시후 다시 시도해주세요.", "Error!",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("로그인 실패.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            EnableUI();
        }

        private void WhenReceiveRegisterResult(RegisterResults result)
        {
            this.timer_update.Stop();


            if (result == RegisterResults.Success)
            {
                MessageBox.Show("계정등록이 성공하였습니다.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result == RegisterResults.Fail_AlreadyExist)
            {
                MessageBox.Show("해당 계정이 이미 존재합니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result == RegisterResults.Fail_InvalidName)
            {
                MessageBox.Show("유효하지않은 이름 입니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (result == RegisterResults.Fail_InvalidPassword)
            {
                MessageBox.Show("유효하지않은 비밀번호 입니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            EnableUI();
        }
    }
}
