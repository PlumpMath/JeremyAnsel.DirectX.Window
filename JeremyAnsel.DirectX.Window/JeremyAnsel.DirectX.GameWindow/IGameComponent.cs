﻿// <copyright file="IGameComponent.cs" company="Jérémy Ansel">
// Copyright (c) 2015 Jérémy Ansel
// </copyright>

namespace JeremyAnsel.DirectX.GameWindow
{
    public interface IGameComponent
    {
        void CreateDeviceDependentResources(DeviceResources resources);

        void ReleaseDeviceDependentResources();

        void CreateWindowSizeDependentResources();

        void Update(StepTimer timer);

        void Render();
    }
}
