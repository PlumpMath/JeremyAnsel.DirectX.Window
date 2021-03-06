﻿// <copyright file="FpsTextRenderer.cs" company="Jérémy Ansel">
// Copyright (c) 2015 Jérémy Ansel
// </copyright>

namespace JeremyAnsel.DirectX.GameWindow
{
    using System;
    using System.Text;
    using JeremyAnsel.DirectX.D2D1;
    using JeremyAnsel.DirectX.DWrite;
    using JeremyAnsel.DirectX.Window;

    public sealed class FpsTextRenderer : IGameComponent
    {
        private DeviceResources deviceResources;

        private WindowPerformanceTime performanceTime;

        private D2D1DrawingStateBlock stateBlock;

        private DWriteTextFormat textFormat;

        private D2D1SolidColorBrush whiteBrush;

        private DWriteTextLayout textLayout;

        private StringBuilder text;

        private bool isInitialized;

        public FpsTextRenderer(WindowPerformanceTime performanceTime)
        {
            if (performanceTime == null)
            {
                throw new ArgumentNullException("performanceTime");
            }

            this.performanceTime = performanceTime;

            this.text = new StringBuilder();

            this.IsEnabled = true;
            this.ShowTime = true;
            this.ShowPerformanceTime = true;
        }

        public bool IsEnabled { get; set; }

        public bool ShowTime { get; set; }

        public bool ShowPerformanceTime { get; set; }

        public void CreateDeviceDependentResources(DeviceResources resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            this.deviceResources = resources;

            this.stateBlock = this.deviceResources.D2DFactory.CreateDrawingStateBlock();

            this.textFormat = this.deviceResources.DWriteFactory.CreateTextFormat("Segoe UI", null, DWriteFontWeight.Light, DWriteFontStyle.Normal, DWriteFontStretch.Normal, 20.0f, string.Empty);
        }

        public void ReleaseDeviceDependentResources()
        {
            D2D1Utils.DisposeAndNull(ref this.stateBlock);
            DWriteUtils.DisposeAndNull(ref this.textFormat);
        }

        public void CreateWindowSizeDependentResources()
        {
            this.whiteBrush = this.deviceResources.D2DRenderTarget.CreateSolidColorBrush(new D2D1ColorF(D2D1KnownColor.White));
        }

        public void Update(StepTimer timer)
        {
            if (!this.IsEnabled)
            {
                return;
            }

            if (timer == null)
            {
                throw new ArgumentNullException("timer");
            }

            var time = timer.TotalTime;
            uint fps = timer.FramesPerSecond;

            text.Clear();

            if (this.ShowPerformanceTime)
            {
                text.Append("E:");
                text.Append(this.performanceTime.EventTime);
                text.Append(", U:");
                text.Append(this.performanceTime.UpdateTime);
                text.Append(", R:");
                text.Append(this.performanceTime.RenderTime);
                text.Append(", P:");
                text.Append(this.performanceTime.PresentTime);
                text.Append("\n");
            }

            if (this.ShowTime)
            {
                text.Append(time);
                text.Append("\n");
            }

            if (fps > 0)
            {
                text.Append(fps);
            }
            else
            {
                text.Append("-");
            }

            text.Append(" FPS");

            DWriteUtils.DisposeAndNull(ref this.textLayout);
            this.textLayout = this.deviceResources.DWriteFactory.CreateTextLayout(text.ToString(), this.textFormat, 600, 400);

            this.isInitialized = true;
        }

        public void Render()
        {
            if (!this.IsEnabled || !this.isInitialized)
            {
                return;
            }

            var context = this.deviceResources.D2DRenderTarget;

            context.SaveDrawingState(this.stateBlock);
            context.BeginDraw();

            this.deviceResources.D2DRenderTarget.DrawTextLayout(new D2D1Point2F(), this.textLayout, this.whiteBrush);

            context.EndDrawIgnoringRecreateTargetError();
            context.RestoreDrawingState(this.stateBlock);
        }
    }
}
