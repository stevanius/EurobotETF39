using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Common
{
    public class Robot : Element
    {
        public float rot, r;
        public bool ready, servosReady;
        public int upToDate = 0;
        public SerialComm comm;
        int statusBits;
        public float rotationConst;

        public Robot(float x, float y, float r)
            : base(x, y)
        {
            this.rot = 0;
            this.r = r;
        }

        public Robot(float x, float y, float r, string portName, bool setPosition = true, bool smallBot = false)
            : base(x, y)
        {
            this.rot = 0;
            this.r = r;
            this.rotationConst = (smallBot ? 16.03f : 15.0f);

            comm = new SerialComm(this, portName);
            if (setPosition) comm.SetPosition(x, y);
        }

        public void Ping()
        {
            CommBuffer buffer = comm.outputBuffer;

            buffer.Write(0xFF);
            buffer.Write(0x0A);
            buffer.Write(0x03);
            buffer.Write(0x08);
            buffer.Write(0x00);

            comm.SendMessage();
        }

        public void PingServos()
        {
            CommBuffer buffer = comm.outputBuffer;

            buffer.Write(0xFF);
            buffer.Write(0x09);
            buffer.Write(0x03);
            buffer.Write(0x08);
            buffer.Write(0x00);

            comm.SendMessage();
        }

        public void SetState(float x, float y, float rot, bool ready, int statusBits)
        {
            this.x = x;
            this.y = y;
            this.rot = rot;
            this.ready = ready;
            this.statusBits = statusBits;
        }

        public bool GetStatusBit(int index)
        {
            return Convert.ToBoolean(statusBits & (0x80000000 >> index));
        }

        public void SetServoState(bool servosReady)
        {
            this.servosReady = servosReady;
        }

        public override void Draw(Graphics g, Camera cam)
        {
            PointF pos = cam.WorldToScreen(x, y);
            PointF aim = cam.WorldToScreen((float)(x - Math.Sin(-rot / 180 * Math.PI) * r), (float)(y + Math.Cos(-rot / 180 * Math.PI) * r));
            float size = cam.Scale(r);
            g.FillEllipse(Brushes.Blue, pos.X - size, pos.Y - size, size * 2, size * 2);

            Pen p = new Pen(Color.Yellow, cam.Scale(10));
            p.EndCap = LineCap.ArrowAnchor;
            g.DrawLine(p, pos.X, pos.Y, aim.X, aim.Y);
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public string State()
        {
            string ret = "";

            ret += x.ToString("00000.00", CultureInfo.InvariantCulture);
            ret += " | ";
            ret += y.ToString("00000.00", CultureInfo.InvariantCulture);
            ret += " | ";
            ret += rot.ToString("00000.00", CultureInfo.InvariantCulture);
            ret += " | ";
            ret += ready;

            return ret;
        }

        bool ignoreMovement = false, ignoreServos = false;

        public void DisableMovement()
        {
            ignoreMovement = true;
        }

        public void DisableServos()
        {
            ignoreServos = true;
        }

        bool MovementReady
        {
            get
            {
                return ready || ignoreMovement;
            }
        }

        bool ServosReady
        {
            get
            {
                return /*servosReady*/ true || ignoreServos;
            }
        }

        public bool Ready
        {
            get { return MovementReady && ServosReady && (upToDate > 4 || ignoreMovement); }
        }

        public void Kill()
        {
            comm.Kill();
        }
    }
}
