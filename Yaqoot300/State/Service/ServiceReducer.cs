﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yaqoot300.Commons;
using Yaqoot300.Interfaces;
using Yaqoot300.Models.Signal;
using Yaqoot300.State.Service;
using Yaqoot300.State.Service.Actions;

namespace Yaqoot300.State.Service
{
    public class ServiceReducer : IReducer<ServiceState>
    {
        public void Reduce(ServiceState state, IAction action)
        {
            switch (action.Type)
            {
                case ServiceActionTypes.CHANGE_M0:
                    var changeM0Payload = ((ServiceChangeM0Action)action).Payload;
                    if (changeM0Payload.Status != null)
                    {
                        state.Motors.M0.Status = changeM0Payload.Status.Value;
                        switch (changeM0Payload.Status.Value)
                        {
                            case RotateMotorStatus.Started:
                                Services.Signals.Send(GuiSignals.ServiceM0Run);
                                break;
                            case RotateMotorStatus.Stopped:
                                Services.Signals.Send(GuiSignals.ServiceM0Stop);
                                break;
                        }
                    }
                    if (changeM0Payload.IsEnabled != null) state.Motors.M0.IsEnabled = changeM0Payload.IsEnabled.Value;
                    break;

                case ServiceActionTypes.CHANGE_M1:
                    var changeM1Payload = ((ServiceChangeM1Action)action).Payload;
                    if (changeM1Payload.Status != null)
                    {
                        state.Motors.M1.Status = changeM1Payload.Status.Value;
                        switch (changeM1Payload.Status.Value)
                        {
                            case RotateMotorStatus.Started:
                                Services.Signals.Send(GuiSignals.ServiceM1Run);
                                break;
                            case RotateMotorStatus.Stopped:
                                Services.Signals.Send(GuiSignals.ServiceM1Stop);
                                break;
                        }
                    }
                    if (changeM1Payload.IsEnabled != null) state.Motors.M1.IsEnabled = changeM1Payload.IsEnabled.Value;
                    break;

                case ServiceActionTypes.CHANGE_M2:
                    var changeM2Payload = ((ServiceChangeM2Action)action).Payload;
                    if (changeM2Payload.Status != null)
                    {
                        state.Motors.M2.Status = changeM2Payload.Status.Value;
                        switch (changeM2Payload.Status.Value)
                        {
                            case RotateMotorStatus.Started:
                                Services.Signals.Send(GuiSignals.ServiceM2Run);
                                break;
                            case RotateMotorStatus.Stopped:
                                Services.Signals.Send(GuiSignals.ServiceM2Stop);
                                break;
                        }
                    }
                    if (changeM2Payload.IsEnabled != null) state.Motors.M2.IsEnabled = changeM2Payload.IsEnabled.Value;
                    break;

                case ServiceActionTypes.CHANGE_M3:
                    var changeM3Payload = ((ServiceChangeM3Action)action).Payload;
                    if (changeM3Payload.Status != null)
                    {
                        state.Motors.M3.Status = changeM3Payload.Status.Value;
                    }
                    if (changeM3Payload.IsEnabled != null) state.Motors.M3.IsEnabled = changeM3Payload.IsEnabled.Value;
                    break;

                case ServiceActionTypes.CHANGE_M4:
                    var changeM4Payload = ((ServiceChangeM4Action)action).Payload;
                    if (changeM4Payload.Status != null)
                    {
                        state.Motors.M4.Status = changeM4Payload.Status.Value;
                        switch (changeM4Payload.Status)
                        {
                            case UpDownMotorStatus.GoingUp:
                                Services.Signals.Send(GuiSignals.ServiceM4Up);
                                break;
                            case UpDownMotorStatus.GoingDown:
                                Services.Signals.Send(GuiSignals.ServiceM4Down);
                                break;
                            case UpDownMotorStatus.Idle:
                                Services.Signals.Send(GuiSignals.ServiceM4Stop);
                                break;
                        }
                    }
                    if (changeM4Payload.IsUpEnabled != null) state.Motors.M4.IsUpEnabled = changeM4Payload.IsUpEnabled.Value;
                    if (changeM4Payload.IsDownEnabled != null) state.Motors.M4.IsDownEnabled = changeM4Payload.IsDownEnabled.Value;
                    break;

                case ServiceActionTypes.CHANGE_SETTINGS:
                    var changeSettingsPayload = ((ServiceChangeSettingsAction)action).Payload;
                    state.PendingSettings = changeSettingsPayload;
                    bool sent = true;
                    if (changeSettingsPayload.ActiveReaders.HasValue)
                    {
                        sent = Services.Signals.Send(GuiSignals.ServiceNumOfActiveReaders, (byte) changeSettingsPayload.ActiveReaders);
                    }
                    if (changeSettingsPayload.FeedInSteps.HasValue)
                    {
                        sent = Services.Signals.Send(GuiSignals.ServiceNumOfFeedInStep, (byte) changeSettingsPayload.FeedInSteps);
                    }
                    if (changeSettingsPayload.M3StepLength.HasValue)
                    {
                        sent = Services.Signals.Send(GuiSignals.ServiceM3StepLength, (byte)changeSettingsPayload.M3StepLength);

                    }
                    if (changeSettingsPayload.M3Speed.HasValue)
                    {
                        sent = Services.Signals.Send(GuiSignals.ServiceM3Speed, (byte)changeSettingsPayload.M3Speed);
                    }
                    if (changeSettingsPayload.M4Speed.HasValue)
                    {
                        sent = Services.Signals.Send(GuiSignals.ServiceM4Speed, (byte) changeSettingsPayload.M4Speed);
                    }
                    if(sent)
                        Services.Store.Dispatch(new ServiceChangeSettingsSuccessAction());
                    else
                        Services.Store.Dispatch(new ServiceChangeSettingsFailAction("Error sending signal"));                      
                    break;

                case ServiceActionTypes.CHANGE_SETTINGS_SUCCESS:
                    if (state.PendingSettings.ActiveReaders.HasValue)
                        state.Settings.ActiveReaders = state.PendingSettings.ActiveReaders.Value;
                    if (state.PendingSettings.FeedInSteps.HasValue)
                        state.Settings.FeedInSteps = state.PendingSettings.FeedInSteps.Value;
                    if (state.PendingSettings.M3StepLength.HasValue)
                        state.Settings.M3StepLength = state.PendingSettings.M3StepLength.Value;
                    if (state.PendingSettings.M3Speed.HasValue)
                        state.Settings.M3Speed = state.PendingSettings.M3Speed.Value;
                    if (state.PendingSettings.M4Speed.HasValue)
                        state.Settings.M4Speed = state.PendingSettings.M4Speed.Value;
                    state.PendingSettings = null;
                    break;

                case ServiceActionTypes.CHANGE_SETTINGS_FAIL:
                    var changeSettingsFailedPayload = ((ServiceChangeSettingsFailAction)action).Payload;
                    state.PendingSettings = null;
                    Services.Messages.Error("Change PLC Settings Failed" +
                                            (string.IsNullOrEmpty(changeSettingsFailedPayload) ? "" : $", {changeSettingsFailedPayload}"), MessageCategory.PLC);
                    break;

                case ServiceActionTypes.TEST_READERS:
                    for (int i = 0; i < state.TestReaders.Readers.Length; i++)
                        state.TestReaders.Readers[i] = TestReaderStatus.Connecting;
                    Task.Run(() =>
                    {
                        Thread.Sleep(4000);
                        var readersStatus = new TestReaderStatus[Constants.READERS_COUNT];
                        for (int i = 0; i < Constants.READERS_COUNT; i++) readersStatus[i] = TestReaderStatus.Success;
                        readersStatus[15] = TestReaderStatus.Fail;
                        readersStatus[7] = TestReaderStatus.Fail;
                        readersStatus[28] = TestReaderStatus.Off;
                        readersStatus[29] = TestReaderStatus.Off;
                        Services.Store.Dispatch(new ServiceTestReadersSuccessAction(readersStatus));
                    });
                    break;

                case ServiceActionTypes.TEST_READERS_SUCCESS:
                    var testReadersSuccessPayload = ((ServiceTestReadersSuccessAction)action).Payload;
                    state.TestReaders.Readers = testReadersSuccessPayload;
                    break;

                case ServiceActionTypes.TEST_READERS_FAIL:
                    state.TestReaders.Readers = new TestReaderStatus[Constants.READERS_COUNT];
                    Services.Messages.Error("Test Readers Failed", MessageCategory.Reader);
                    break;

                case ServiceActionTypes.ASSIGN_READER:
                    var assginReaderPayload = ((ServiceAssignReaderAction)action).Payload;
                    var reader =
                        state.SetupReaders.Readers.FirstOrDefault(r => r.ReaderName == assginReaderPayload.ReaderName);
                    if (reader != null) reader.ReaderNumber = assginReaderPayload.ReaderNumber;
                    break;

                case ServiceActionTypes.SENSORS_CHANGED:
                    var sensorsChangedPayload = ((ServiceSensorsChangedAction)action).Payload;
                    for (int i = sensorsChangedPayload.StartIndex; i <= sensorsChangedPayload.EndIndex && i < Constants.SENSORS_LENGTH; i++)
                    {
                        state.Sensors[i] = sensorsChangedPayload.Sensors[i - sensorsChangedPayload.StartIndex];
                    }
                    break;
            }
        }
    }
}
