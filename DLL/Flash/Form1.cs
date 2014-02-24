using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FlashTest
{
    public partial class Form1 : Form
    {
        private const string METHODSTRING = "<invoke name=\"{0}\" returntype=\"xml\"><arguments><string>{1}</string></arguments></invoke>";
        private const string METHODEMPTY = "<invoke name=\"{0}\" returntype=\"xml\"><arguments></arguments></invoke>";
        //private const string METHODBOOL = "<invoke name=\"{0}\" returntype=\"xml\"><arguments><Boolean>{1}</Boolean></arguments></invoke>";
        private const string METHODNUM = "<invoke name=\"{0}\" returntype=\"xml\"><arguments><Number>{1}</Number></arguments></invoke>";
        private const string METHODSETGAMESTATUS = "<invoke name=\"SetGameStatus\" returntype=\"xml\"><arguments><string>{0}</string><string>{1}</string><string>{2}</string><string>{3}</string><string>{4}</string><string>{5}</string><string>{6}</string></arguments></invoke>";

        int _ball = 1;
        int _playerIndex = 0;
        long _p1Score = 0;
        long _p2Score = 0;
        long _p3Score = 0;
        long _p4Score = 0;
        int _playerUp = 1;
        int _numPlayers = 1;
        int _weapon = 0;
        int _magic = 0;
        bool _hasShield = false;
        bool _hasCross = false;

        public Form1()
        {
            InitializeComponent();

            axShockwaveFlash1.FlashCall +=
                new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(
                    axShockwaveFlash1_FlashCall);

            axShockwaveFlash2.FlashCall +=
                new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(
                    axShockwaveFlash2_FlashCall);

            //axShockwaveFlash1.LoadMovie(Application.StartupPath + "\\cmm.swf");
            axShockwaveFlash1.LoadMovie(0, @"C:\Documents and Settings\fernando\My Documents\Cabinet\Flash\CMM.swf");
        }

        void axShockwaveFlash1_FlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
                Console.WriteLine(e.request);
        }

        void axShockwaveFlash2_FlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            string text = e.request;
            if (text.Contains("FINISHEDMULTIBALL") || 
                text.Contains("FINISHEDBALLLOCK") || 
                text.Contains("FINISHEDJACKPOT"))
            {
                axShockwaveFlash2.Visible = false;
            }
            Console.WriteLine(text);
        }

        private void cmdBall_Click(object sender, EventArgs e)
        {
            _ball++;
            if (_ball > 5)
                _ball = 1;
            callFlashString("SetBallCount", _ball);
        }

        private void cmdWeapon_Click(object sender, EventArgs e)
        {
            _weapon++;
            if (_weapon > 3)
                _weapon = 0;
            callFlashString("SetWeaponLevel", _weapon);
        }

        private void cmdMagic_Click(object sender, EventArgs e)
        {
            _magic++;
            if (_magic > 3)
                _magic = 0;
            callFlashString("SetMagicLevel", _magic);
        }

        private void cmdCross_Click(object sender, EventArgs e)
        {
            _hasCross = !_hasCross;
            callFlashString("SetCross", _hasCross ? "1" : "0");
        }

        private void CmdShield_Click(object sender, EventArgs e)
        {
            _hasShield = !_hasShield;

            callFlashString("SetShield", _hasShield ? "1" : "0");
        }

        private void cmdColor_Click(object sender, EventArgs e)
        {
            callFlashString("SetColor", ((Button)sender).Text);
            callFlashString("SetRoom", ((Button)sender).Text);
            callFlashString("SetRoomName", ((Button)sender).Text);
        }

        private void cmdShowJackpot_Click(object sender, EventArgs e)
        {
            if (axShockwaveFlash2.Visible)
                return;
            axShockwaveFlash2.LoadMovie(0, " ");
            axShockwaveFlash2.Visible = true;
            axShockwaveFlash2.LoadMovie(0, @"C:\Documents and Settings\fernando\My Documents\Cabinet\Flash\Multiball.Succubus.swf");
            callFlashString2("ShowJackpot", "party");
        }

        private string callFlashString(string method, object val)
        {
            string response = string.Empty;

            try
            {
                response = axShockwaveFlash1.CallFunction(string.Format(METHODSTRING, method, val));
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return response;
        }

        private string callFlashString2(string method, object val)
        {
            string response = string.Empty;

            try
            {
                response = axShockwaveFlash2.CallFunction(string.Format(METHODSTRING, method, val));
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return response;
        }

        private string callFlashEmpty2(string method)
        {
            string response = string.Empty;

            try
            {
                response = axShockwaveFlash2.CallFunction(string.Format(METHODEMPTY, method));
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return response;
        }

        private void setGameStatus()
        {
            try
            {
                string response = axShockwaveFlash1.CallFunction(string.Format(METHODSETGAMESTATUS, 
                    _ball,
                    _p1Score,
                    _p2Score,
                    _p3Score,
                    _p4Score,
                    _playerUp,
                    _numPlayers));
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void cmdSetGameStatus_Click(object sender, EventArgs e)
        {
            _ball = (int)numBall.Value;
            _p1Score = (long)numP1.Value;
            _p2Score = (long)numP2.Value;
            _p3Score = (long)numP3.Value;
            _p4Score = (long)numP4.Value;
            _playerUp = (int)numPlayerUp.Value;
            _numPlayers = (int)numPlayers.Value;
            setGameStatus();
        }

        private void cmdLockBall_Click(object sender, EventArgs e)
        {
            if (axShockwaveFlash2.Visible)
                return;
            axShockwaveFlash2.LoadMovie(0, " ");
            axShockwaveFlash2.Visible = true;
            axShockwaveFlash2.LoadMovie(0, @"C:\Documents and Settings\fernando\My Documents\Cabinet\Flash\Multiball.Succubus.swf");
            callFlashString2("ShowBallLock", "2");
        }

        private void cmdSuccubusMultiball_Click(object sender, EventArgs e)
        {
            if (axShockwaveFlash2.Visible)
                return;
            axShockwaveFlash2.LoadMovie(0, " ");
            axShockwaveFlash2.Visible = true;
            axShockwaveFlash2.LoadMovie(0, @"C:\Documents and Settings\fernando\My Documents\Cabinet\Flash\Multiball.Succubus.swf");
            callFlashEmpty2("ShowMultiball");
        }
    }
}