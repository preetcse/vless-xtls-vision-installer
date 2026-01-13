using System;
using System.Collections.Generic;
using GTA;
using GTA.UI;
using System.Windows.Forms;
using RoleplayOverhaul.Jobs;

namespace RoleplayOverhaul.UI
{
    public class JobMenu
    {
        private JobManager _jobManager;
        private List<IJob> _jobs;
        private bool _isVisible;
        private int _selectedIndex;

        public JobMenu(JobManager jobManager)
        {
            _jobManager = jobManager;
            // Hack to access internal list, or just pass the list from Main
            // Ideally JobManager has GetJobs().
            // I'll assume I can pass the list or get it.
            // For now, empty list until SetJobs called.
            _jobs = new List<IJob>();
        }

        public void SetJobs(List<IJob> jobs)
        {
            _jobs = jobs;
        }

        public void Toggle()
        {
            _isVisible = !_isVisible;
            Game.Player.CanControlCharacter = !_isVisible;
        }

        public void HandleInput(KeyEventArgs e)
        {
            if (!_isVisible) return;

            if (e.KeyCode == Keys.Down) _selectedIndex = Math.Min(_selectedIndex + 1, _jobs.Count - 1);
            if (e.KeyCode == Keys.Up) _selectedIndex = Math.Max(_selectedIndex - 1, 0);

            if (e.KeyCode == Keys.Enter)
            {
                var job = _jobs[_selectedIndex];
                if (job.IsActive) job.End();
                else job.Start();
                Toggle();
            }
        }

        public void Draw()
        {
            if (!_isVisible) return;

            // Draw Background
            new ContainerElement(new System.Drawing.PointF(100, 100), new System.Drawing.SizeF(400, 600), System.Drawing.Color.FromArgb(200, 0, 0, 0)).Draw();
            new TextElement("JOB CENTER", new System.Drawing.PointF(120, 120), 0.8f, System.Drawing.Color.White).Draw();

            for (int i = 0; i < _jobs.Count; i++)
            {
                if (i < _selectedIndex - 10 || i > _selectedIndex + 10) continue; // Simple scrolling

                var job = _jobs[i];
                float y = 180 + ((i - Math.Max(0, _selectedIndex-10)) * 40);
                System.Drawing.Color color = (i == _selectedIndex) ? System.Drawing.Color.Yellow : System.Drawing.Color.White;
                string status = job.IsActive ? "[ACTIVE]" : "";

                new TextElement($"{job.Name} {status}", new System.Drawing.PointF(120, y), 0.5f, color).Draw();
            }
        }
    }
}
