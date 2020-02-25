using SMS.Client.Log;
using SMS.StreamMedia.ClientSDK.Helpers;
using SMS.StreamMedia.ClientSDK.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SMS.StreamMedia.ClientSDK
{
    public class SMClient : IDisposable
    {
        #region Fields

        private static readonly object _lock = new object();
        private static SMClient _instance = null;

        private ConcurrentDictionary<string, DevLoginModel> _loginedDeviceDict = null;

        private CHCNetSDK.REALDATACALLBACK _HCRealDataCallback = null;
        private CHCNetSDK.MSGCallBack_V31 _msgCallback_V31 = null;

        private event Action<int, CHCNetSDK.NET_DVR_ALARMER, IntPtr, uint, IntPtr> OnUploadAlarm;

        #endregion

        #region Properties

        public static SMClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SMClient();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Constructors

        private SMClient()
        {
            Init();
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            _loginedDeviceDict = new ConcurrentDictionary<string, DevLoginModel>();
            _HCRealDataCallback = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);

            bool isInit = CHCNetSDK.NET_DVR_Init();
        }

        private string GetDeviceLoginUserDictKey(string devIp, int devPort, string userName, string password)
        {
            string loginUserKey = string.Format("{0}-{1}-{2}-{3}", devIp, devPort, userName, password);
            return loginUserKey;
        }

        /// <summary>
        /// 设备登陆
        /// </summary>
        /// <param name="devIp"></param>
        /// <param name="devPort"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private DevLoginModel DeviceLogin(string devIp, int devPort, string userName, string password)
        {
            string loginUserKey = GetDeviceLoginUserDictKey(devIp, devPort, userName, password);

            //设备已经登陆，择直接返回登陆id，并增加1此登陆的订阅数
            if (_loginedDeviceDict.ContainsKey(loginUserKey))
            {
                _loginedDeviceDict[loginUserKey].SubscriberCount++;
                return _loginedDeviceDict[loginUserKey];
            }

            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            //登录设备 Login the device
            int loginId = CHCNetSDK.NET_DVR_Login_V30(devIp, devPort, userName, password, ref DeviceInfo);
            if (loginId < 0)
            {
                uint lastErr = CHCNetSDK.NET_DVR_GetLastError();
                LogHelper.Debug(string.Format("设备{0}-{1}登陆失败，错误代码[{2}]", devIp, devPort, lastErr));
                return null;
            }

            //设置报警回调函数
            if (_msgCallback_V31 == null)
            {
                _msgCallback_V31 = new CHCNetSDK.MSGCallBack_V31(MsgCallback_V31);
            }

            RuntimeTypeHandle handle = GetType().TypeHandle;
            IntPtr ptr = handle.Value;
            if (!CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(_msgCallback_V31, ptr))
            {
                uint lastErr = CHCNetSDK.NET_DVR_GetLastError();
                LogHelper.Debug(string.Format("设备{0}-{1}设置报警回调失败，错误代码[{2}]", devIp, devPort, lastErr));
            }

            DevLoginModel devLoginModel = new DevLoginModel();
            devLoginModel.LoginId = loginId;
            devLoginModel.SubscriberCount = 1;

            _loginedDeviceDict.TryAdd(loginUserKey, devLoginModel);
            return devLoginModel;
        }

        private bool MsgCallback_V31(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            AlarmMessageHandle(lCommand, ref pAlarmer, pAlarmInfo, dwBufLen, pUser);

            return true; //回调函数需要有返回，表示正常接收到数据
        }

        public void AlarmMessageHandle(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            OnUploadAlarm.Invoke(lCommand, pAlarmer, pAlarmInfo, dwBufLen, pUser);
        }

        /// <summary>
        /// 注销登陆
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>成功返回0，失败返回错误码</returns>
        private uint DeviceLogout(int userId)
        {
            KeyValuePair<string, DevLoginModel> loginModelKV = _loginedDeviceDict.FirstOrDefault(p => p.Value.LoginId == userId);
            if (loginModelKV.Value != null)
            {
                loginModelKV.Value.SubscriberCount--;
                if (loginModelKV.Value.SubscriberCount <= 0)
                {
                    DevLoginModel removeModel = null;
                    _loginedDeviceDict.TryRemove(loginModelKV.Key, out removeModel);
                }
            }

            if (!CHCNetSDK.NET_DVR_Logout(userId))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();

                LogHelper.Debug(string.Format("设备{0}注销失败，错误代码[{1}]", userId, iLastErr));
                return iLastErr;
            }

            return 0;
        }

        private void RealDataCallBack(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser)
        {
            if (dwBufSize > 0)
            {
                //byte[] sData = new byte[dwBufSize];
                //Marshal.Copy(pBuffer, sData, 0, (Int32)dwBufSize);

                //string str = "实时流数据.ps";
                //FileStream fs = new FileStream(str, FileMode.Create);
                //int iLen = (int)dwBufSize;
                //fs.Write(sData, 0, iLen);
                //fs.Close();
            }
        }

        #endregion

        #region Public Methods

        #region Preview

        /// <summary>
        /// 开始预览
        /// </summary>
        /// <param name="screenHandle"></param>
        /// <param name="devIp"></param>
        /// <param name="devPort"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="channel"></param>
        /// <param name="stremType"></param>
        /// <returns></returns>
        public int StartPreview(IntPtr screenHandle, string devIp, int devPort, string userName, string password, int channel, uint stremType)
        {
            int userId = -1;

            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            if (userId < 0)
            {
                return -1;
            }

            CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            lpPreviewInfo.hPlayWnd = screenHandle;//预览窗口
            lpPreviewInfo.lChannel = channel;//预te览的设备通道
            lpPreviewInfo.dwStreamType = stremType;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
            lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数
            lpPreviewInfo.byProtoType = 0;
            lpPreviewInfo.byPreviewMode = 0;

            IntPtr pUser = new IntPtr();
            int realPlayId = CHCNetSDK.NET_DVR_RealPlay_V40(userId, ref lpPreviewInfo, _HCRealDataCallback, pUser);
            if (realPlayId < 0)
            {
                uint lastErr = CHCNetSDK.NET_DVR_GetLastError();
                LogHelper.Debug(string.Format("设备{0}-{1}-{2}实时预览失败，错误代码[{3}]", devIp, devPort, channel, lastErr));
                return Math.Abs((int)lastErr) * (-1);
            }

            return realPlayId;
        }

        /// <summary>
        /// 停止预览，操作成功返回0，失败返回错误码
        /// </summary>
        /// <param name="playHandle"></param>
        /// <returns></returns>
        public int StopPreview(int playHandle)
        {
            if (!CHCNetSDK.NET_DVR_StopRealPlay(playHandle))
            {
                uint lastErr = CHCNetSDK.NET_DVR_GetLastError();

                LogHelper.Debug(string.Format("设备{0}停止实时预览失败，错误代码[{1}]", playHandle, lastErr));
                return Math.Abs((int)lastErr) * (-1);
            }

            return 0;
        }

        /// <summary>
        /// 抓bmp图
        /// </summary>
        /// <param name="playHandle"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int CapturePictureBMP(int playHandle, string fileName)
        {
            //BMP抓图 Capture a BMP picture
            if (!CHCNetSDK.NET_DVR_CapturePicture(playHandle, fileName))
            {
                uint lastErr = CHCNetSDK.NET_DVR_GetLastError();
                LogHelper.Debug(string.Format("设备{0}预览抓图失败，错误代码[{1}]", playHandle, lastErr));
                return Math.Abs((int)lastErr) * (-1);
            }

            return 0;
        }

        /// <summary>
        /// 抓jpeg图
        /// </summary>
        /// <param name="loginId"></param>
        /// <param name="channel"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int CapturePictureJPEG(string devIp, int devPort, string userName, string password, int channel, string fileName)
        {
            int userId = -1;

            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            if (userId < 0)
            {
                return -1;
            }

            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
            //图像质量 Image quality
            lpJpegPara.wPicQuality = 0;
            //抓图分辨率 Picture size: 2- 4CIF，0xff- Auto(使用当前码流分辨率)，抓图分辨率需要设备支持，更多取值请参考SDK文档
            lpJpegPara.wPicSize = 0xff;

            //JPEG抓图 Capture a JPEG picture
            if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(userId, channel, ref lpJpegPara, fileName))
            {
                uint lastErr = CHCNetSDK.NET_DVR_GetLastError();
                LogHelper.Debug(string.Format("设备{0}预览抓图失败，错误代码[{1}]", userId, lastErr));
                return Math.Abs((int)lastErr) * (-1);
            }

            return 0;
        }

        /// <summary>
        /// 开始录像
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="channel"></param>
        /// <param name="loginId"></param>
        /// <param name="playHandle"></param>
        /// <returns></returns>
        public int StartRecord(string fileName, string devIp, int devPort, string userName, string password, int channel, int playHandle)
        {
            int userId = -1;

            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            if (userId < 0)
            {
                return -1;
            }

            //强制I帧 Make a I frame
            CHCNetSDK.NET_DVR_MakeKeyFrame(userId, channel);

            //开始录像 Start recording
            if (!CHCNetSDK.NET_DVR_SaveRealData(playHandle, fileName))
            {
                uint lastErr = CHCNetSDK.NET_DVR_GetLastError();
                LogHelper.Debug(string.Format("设备{0}预览开始录像失败，错误代码[{1}]", playHandle, lastErr));
                return Math.Abs((int)lastErr) * (-1);
            }

            return 0;
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        /// <param name="playHandle"></param>
        /// <returns></returns>
        public int StopRecord(int playHandle)
        {
            if (!CHCNetSDK.NET_DVR_StopSaveRealData(playHandle))
            {
                uint lastErr = CHCNetSDK.NET_DVR_GetLastError();
                LogHelper.Debug(string.Format("设备{0}预览停止录像失败，错误代码[{1}]", playHandle, lastErr));
                return Math.Abs((int)lastErr) * (-1);
            }

            return 0;
        }

        /// <summary>
        /// 云台控制
        /// </summary>
        /// <param name="playHandle"></param>
        /// <param name="controlType"></param>
        /// <param name="stopFlag"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public bool PTZControl(int playHandle, SMPTZControlType controlType, int stopFlag, int speed)
        {
            int ptzControlInt = -1;
            switch (controlType)
            {
                case SMPTZControlType.Up:
                    ptzControlInt = CHCNetSDK.TILT_UP;
                    break;
                case SMPTZControlType.Down:
                    ptzControlInt = CHCNetSDK.TILT_DOWN;
                    break;
                case SMPTZControlType.Left:
                    ptzControlInt = CHCNetSDK.PAN_LEFT;
                    break;
                case SMPTZControlType.Right:
                    ptzControlInt = CHCNetSDK.PAN_RIGHT;
                    break;
                case SMPTZControlType.UpLeft:
                    ptzControlInt = CHCNetSDK.UP_LEFT;
                    break;
                case SMPTZControlType.UpRight:
                    ptzControlInt = CHCNetSDK.UP_RIGHT;
                    break;
                case SMPTZControlType.DownLeft:
                    ptzControlInt = CHCNetSDK.DOWN_LEFT;
                    break;
                case SMPTZControlType.DownRight:
                    ptzControlInt = CHCNetSDK.DOWN_RIGHT;
                    break;
                case SMPTZControlType.ZoomIn:
                    ptzControlInt = CHCNetSDK.ZOOM_IN;
                    break;
                case SMPTZControlType.ZoomOut:
                    ptzControlInt = CHCNetSDK.ZOOM_OUT;
                    break;
            }

            if (ptzControlInt == -1)
            {
                return false;
            }

            return CHCNetSDK.NET_DVR_PTZControlWithSpeed(playHandle, (uint)ptzControlInt, (uint)stopFlag, (uint)speed);
        }

        #endregion

        #region Replay

        public List<DateTime> SearchRecordDate(string devIp, int devPort, string userName, string password, int channel, DateTime beginTime, DateTime endTime)
        {
            int userId = -1;

            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            List<DateTime> dateList = new List<DateTime>();
            if (userId < 0)
            {
                return dateList;
            }

            int queryDayCount = (int)(endTime.Date - beginTime.Date).TotalDays + 1;
            for (int i = 0; i < queryDayCount; i++)
            {
                CHCNetSDK.NET_DVR_FILECOND_V40 struFileCond_V40 = new CHCNetSDK.NET_DVR_FILECOND_V40();
                struFileCond_V40.lChannel = channel; //通道号 Channel number
                struFileCond_V40.dwFileType = 0xff; //0xff-全部，0-定时录像，1-移动侦测，2-报警触发，...
                struFileCond_V40.dwIsLocked = 0xff; //0-未锁定文件，1-锁定文件，0xff表示所有文件（包括锁定和未锁定）

                struFileCond_V40.struStartTime = beginTime.Date.AddDays(i).ToHCDeviceTime();
                struFileCond_V40.struStopTime = beginTime.Date.AddDays(i + 1).AddSeconds(-1).ToHCDeviceTime();

                int m_lFindHandle = CHCNetSDK.NET_DVR_FindFile_V40(userId, ref struFileCond_V40);
                if (m_lFindHandle < 0)
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    continue;
                }
                else
                {
                    CHCNetSDK.NET_DVR_FINDDATA_V30 struFileData = new CHCNetSDK.NET_DVR_FINDDATA_V30();
                    while (true)
                    {
                        //逐个获取查找到的文件信息 Get file information one by one.
                        int result = CHCNetSDK.NET_DVR_FindNextFile_V30(m_lFindHandle, ref struFileData);
                        if (result == CHCNetSDK.NET_DVR_ISFINDING)  //正在查找请等待 Searching, please wait
                        {
                            continue;
                        }
                        else if (result == CHCNetSDK.NET_DVR_FILE_SUCCESS) //获取文件信息成功 Get the file information successfully
                        {
                            dateList.Add(beginTime.Date.AddDays(i));
                            break;
                        }
                        else if (result == CHCNetSDK.NET_DVR_FILE_NOFIND || result == CHCNetSDK.NET_DVR_NOMOREFILE)
                        {
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return dateList;
        }

        public List<SMVideoRecordFile> SearchRecordFile(string devIp, int devPort, string userName, string password, int channel, DateTime beginTime, DateTime endTime)
        {
            int userId = -1;

            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            if (userId < 0)
            {
                return null;
            }

            CHCNetSDK.NET_DVR_FILECOND_V40 struFileCond_V40 = new CHCNetSDK.NET_DVR_FILECOND_V40();

            struFileCond_V40.lChannel = channel; //通道号 Channel number
            struFileCond_V40.dwFileType = 0xff; //0xff-全部，0-定时录像，1-移动侦测，2-报警触发，...
            struFileCond_V40.dwIsLocked = 0xff; //0-未锁定文件，1-锁定文件，0xff表示所有文件（包括锁定和未锁定）

            //设置录像查找的开始时间 Set the starting time to search video files
            struFileCond_V40.struStartTime = beginTime.ToHCDeviceTime();

            //设置录像查找的结束时间 Set the stopping time to search video files
            struFileCond_V40.struStopTime = endTime.ToHCDeviceTime();

            //开始录像文件查找 Start to search video files 
            int m_lFindHandle = CHCNetSDK.NET_DVR_FindFile_V40(userId, ref struFileCond_V40);

            if (m_lFindHandle < 0)
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return null;
            }
            else
            {
                CHCNetSDK.NET_DVR_FINDDATA_V30 struFileData = new CHCNetSDK.NET_DVR_FINDDATA_V30();
                List<SMVideoRecordFile> videoRecordFileList = new List<SMVideoRecordFile>();
                while (true)
                {
                    //逐个获取查找到的文件信息 Get file information one by one.
                    int result = CHCNetSDK.NET_DVR_FindNextFile_V30(m_lFindHandle, ref struFileData);

                    if (result == CHCNetSDK.NET_DVR_ISFINDING)  //正在查找请等待 Searching, please wait
                    {
                        continue;
                    }
                    else if (result == CHCNetSDK.NET_DVR_FILE_SUCCESS) //获取文件信息成功 Get the file information successfully
                    {
                        SMVideoRecordFile videoRecord = new SMVideoRecordFile();
                        videoRecord.FileName = struFileData.sFileName;
                        videoRecord.FileSize = (int)struFileData.dwFileSize;
                        videoRecord.BeginTime = struFileData.struStartTime.ToDateTime();
                        videoRecord.EndTime = struFileData.struStopTime.ToDateTime();
                        videoRecordFileList.Add(videoRecord);
                    }
                    else if (result == CHCNetSDK.NET_DVR_FILE_NOFIND || result == CHCNetSDK.NET_DVR_NOMOREFILE)
                    {
                        //未查找到文件或者查找结束，退出 
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                return videoRecordFileList;
            }
        }

        public int StartReplayByFile(IntPtr screenHandle, string devIp, int devPort, string userName, string password, string recordFileName)
        {
            int userId = -1;

            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            if (userId < 0)
            {
                return -1;
            }

            //按文件名回放
            int replayHandle = CHCNetSDK.NET_DVR_PlayBackByName(userId, recordFileName, screenHandle);
            if (replayHandle < 0)
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return Math.Abs((int)iLastErr) * (-1);
            }

            uint iOutValue = 0;
            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(replayHandle, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return Math.Abs((int)iLastErr) * (-1);
            }

            return replayHandle;
        }

        public int StartReplayByTime(IntPtr screenHandle, string devIp, int devPort, string userName, string password, int channelNumber, DateTime beginTime, DateTime endTime)
        {
            int userId = -1;

            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            if (userId < 0)
            {
                return -1;
            }

            CHCNetSDK.NET_DVR_VOD_PARA struVodPara = new CHCNetSDK.NET_DVR_VOD_PARA();
            struVodPara.dwSize = (uint)Marshal.SizeOf(struVodPara);
            struVodPara.struIDInfo.dwChannel = (uint)channelNumber; //通道号 Channel number  
            struVodPara.hWnd = screenHandle;//回放窗口句柄

            //设置回放的开始时间 Set the starting time to search video files
            struVodPara.struBeginTime = beginTime.ToHCDeviceTime();

            //设置回放的结束时间 Set the stopping time to search video files
            struVodPara.struEndTime = endTime.ToHCDeviceTime();

            //按时间回放 Playback by time
            int playHandle = CHCNetSDK.NET_DVR_PlayBackByTime_V40(userId, ref struVodPara);
            if (playHandle < 0)
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return Math.Abs((int)iLastErr) * (-1);
            }

            uint iOutValue = 0;
            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return Math.Abs((int)iLastErr) * (-1);
            }

            return playHandle;
        }

        public int GetReplayPosition(int playHandle)
        {
            uint iOutValue = 0;
            int iPos = 0;

            IntPtr lpOutBuffer = Marshal.AllocHGlobal(4);

            //获取回放进度
            CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYGETPOS, IntPtr.Zero, 0, lpOutBuffer, ref iOutValue);

            iPos = (int)Marshal.PtrToStructure(lpOutBuffer, typeof(int));
            Marshal.FreeHGlobal(lpOutBuffer);

            return iPos;
        }

        public DateTime GetReplayOsdDateTime(int playHandle)
        {
            DateTime osdDateTime = DateTime.MinValue;
            CHCNetSDK.NET_DVR_TIME nvrOsdTime = new CHCNetSDK.NET_DVR_TIME();
            bool isSuccessful = CHCNetSDK.NET_DVR_GetPlayBackOsdTime(playHandle, ref nvrOsdTime);
            if (isSuccessful)
            {
                osdDateTime = nvrOsdTime.ToDateTime();
            }

            return osdDateTime;
        }

        public int SetReplayPosition(int playHandle)
        {
            uint iOutValue = 0;
            int iPos = 0;

            IntPtr lpOutBuffer = Marshal.AllocHGlobal(4);

            //获取回放进度
            CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYGETPOS, IntPtr.Zero, 0, lpOutBuffer, ref iOutValue);

            iPos = (int)Marshal.PtrToStructure(lpOutBuffer, typeof(int));
            Marshal.FreeHGlobal(lpOutBuffer);

            return iPos;
        }

        public bool StopReplay(int playHandle)
        {
            if (playHandle < 0)
            {
                return false;
            }

            //停止回放
            if (!CHCNetSDK.NET_DVR_StopPlayBack(playHandle))
            {
                return false;
            }

            return true;
        }

        public bool PauseReplay(int playHandle, bool isPause)
        {
            uint iOutValue = 0;

            if (isPause)
            {
                if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYPAUSE, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    return false;
                }
            }
            else
            {
                if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYRESTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    return false;
                }
            }

            return true;
        }

        public bool SlowReplay(int playHandle)
        {
            uint iOutValue = 0;

            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYSLOW, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return false;
            }

            return true;
        }

        public bool FastReplay(int playHandle)
        {
            uint iOutValue = 0;
            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYFAST, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return false;
            }
            return true;
        }

        public bool FrameReplay(int playHandle)
        {
            uint iOutValue = 0;

            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYFRAME, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return false;
            }

            return true;
        }

        public bool ResumeReplay(int playHandle)
        {
            uint iOutValue = 0;

            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAYNORMAL, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return false;
            }

            return true;
        }

        public bool ReverseReplay(int playHandle, bool reverse)
        {
            uint iOutValue = 0;
            if (reverse)
            {
                if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAY_REVERSE, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string errMsg = string.Format("NET_DVR_PLAY_REVERSE failed, error code= ", iLastErr);
                    return false;
                }
            }
            else
            {
                if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(playHandle, CHCNetSDK.NET_DVR_PLAY_FORWARD, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
                {
                    uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string errMsg = string.Format("NET_DVR_PLAY_FORWARD failed, error code= ", iLastErr);
                    return false;
                }
            }

            return true;
        }

        public bool ReplayCapturePicture(int replayHandle, string fileName)
        {
            if (replayHandle < 0)
            {
                return false;
            }

            if (!CHCNetSDK.NET_DVR_PlayBackCaptureFile(replayHandle, fileName))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                return false;
            }

            return true;
        }

        #endregion

        #region Download

        public int DownloadVideoRecordByTime(int userId, int channelNumber, DateTime beginTime, DateTime endTime, string fullPath)
        {
            CHCNetSDK.NET_DVR_PLAYCOND struDownPara = new CHCNetSDK.NET_DVR_PLAYCOND();
            //通道号 Channel number  
            struDownPara.dwChannel = (uint)channelNumber;
            //设置下载的开始时间 Set the starting time
            struDownPara.struStartTime = beginTime.ToHCDeviceTime();
            //设置下载的结束时间 Set the stopping time
            struDownPara.struStopTime = endTime.ToHCDeviceTime();

            //按时间下载 Download by time
            int downloadId = CHCNetSDK.NET_DVR_GetFileByTime_V40(userId, fullPath, ref struDownPara);
            if (downloadId < 0)
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_GetFileByTime_V40 failed, error code= " + iLastErr;
                return -1;
            }

            uint iOutValue = 0;
            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(downloadId, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                //下载控制失败，输出错误号
                string str = "NET_DVR_PLAYSTART failed, error code= " + iLastErr;
                return -1;
            }

            return downloadId;
        }

        public int DownloadVideoRecordByFile(int userId, string videoFileName, string fullPath)
        {
            int downloadHandle = CHCNetSDK.NET_DVR_GetFileByName(userId, videoFileName, fullPath);
            if (downloadHandle < 0)
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_GetFileByName failed, error code= " + iLastErr;
                return -1;
            }

            uint iOutValue = 0;
            if (!CHCNetSDK.NET_DVR_PlayBackControl_V40(downloadHandle, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_PLAYSTART failed, error code= " + iLastErr;
                return -1;
            }

            return downloadHandle;
        }

        public bool StopDownload(int downloadHandle)
        {
            if (downloadHandle < 0)
            {
                return false;
            }

            if (!CHCNetSDK.NET_DVR_StopGetFile(downloadHandle))
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_StopGetFile failed, error code= " + iLastErr;

                return false;
            }

            return true;
        }

        public int GetDownloadPosition(int downloadHandle)
        {
            int iPos = CHCNetSDK.NET_DVR_GetDownloadPos(downloadHandle);
            return iPos;
        }

        #endregion

        #region Dev Info

        public PTZInfo GetPTZInfo(string devIp, int devPort, string userName, string password)
        {
            PTZInfo ptzInfo = new PTZInfo();

            int userId = -1;
            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            CHCNetSDK.NET_DVR_PTZPOS ptzPos = new CHCNetSDK.NET_DVR_PTZPOS();
            uint dwReturn = 0;
            int nSize = Marshal.SizeOf(ptzPos);
            IntPtr ptrPtzCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(ptzPos, ptrPtzCfg, false);

            if (CHCNetSDK.NET_DVR_GetDVRConfig(userId, CHCNetSDK.NET_DVR_GET_PTZPOS, -1, ptrPtzCfg, (UInt32)nSize, ref dwReturn))
            {
                ptzPos = (CHCNetSDK.NET_DVR_PTZPOS)Marshal.PtrToStructure(ptrPtzCfg, typeof(CHCNetSDK.NET_DVR_PTZPOS));

                //成功获取显示ptz参数
                ushort wPanPos = Convert.ToUInt16(Convert.ToString(ptzPos.wPanPos, 16));
                ptzInfo.Pan = wPanPos * 0.1f;
                ushort wTiltPos = Convert.ToUInt16(Convert.ToString(ptzPos.wTiltPos, 16));
                ptzInfo.Tilt = wTiltPos * 0.1f;
                ushort wZoomPos = Convert.ToUInt16(Convert.ToString(ptzPos.wZoomPos, 16));
                ptzInfo.Zoom = wZoomPos * 0.1f;
            }

            Marshal.FreeHGlobal(ptrPtzCfg);
            return ptzInfo;
        }

        public CHCNetSDK.NET_DVR_GIS_INFO GetGISInfo(string devIp, int devPort, string userName, string password)
        {
            int userId = -1;
            DevLoginModel devLoginModel = DeviceLogin(devIp, devPort, userName, password);
            if (devLoginModel != null)
            {
                userId = devLoginModel.LoginId;
            }

            CHCNetSDK.NET_DVR_GIS_INFO gis_info_tmp = new CHCNetSDK.NET_DVR_GIS_INFO();
            int nSize = Marshal.SizeOf(gis_info_tmp);
            IntPtr ptrGisInfo = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(gis_info_tmp, ptrGisInfo, false);

            int channel = 1;
            int nCondSize = Marshal.SizeOf(channel);
            IntPtr ptrCondCfg = Marshal.AllocHGlobal(nCondSize);
            Marshal.StructureToPtr(channel, ptrCondCfg, false);

            CHCNetSDK.NET_DVR_STD_CONFIG std_config = new CHCNetSDK.NET_DVR_STD_CONFIG();
            std_config.lpStatusBuffer = IntPtr.Zero;
            std_config.dwStatusSize = 0;
            std_config.lpOutBuffer = ptrGisInfo;
            std_config.dwOutSize = (uint)Marshal.SizeOf(gis_info_tmp);
            std_config.byDataType = 0;
            std_config.lpCondBuffer = ptrCondCfg;
            std_config.dwCondSize = (uint)Marshal.SizeOf(channel);

            bool ret = CHCNetSDK.NET_DVR_GetSTDConfig(userId, CHCNetSDK.NET_DVR_GET_GISINFO, ref std_config);
            if (ret)
            {
                gis_info_tmp = (CHCNetSDK.NET_DVR_GIS_INFO)Marshal.PtrToStructure(ptrGisInfo, typeof(CHCNetSDK.NET_DVR_GIS_INFO));
            }

            Marshal.FreeHGlobal(ptrGisInfo);
            Marshal.FreeHGlobal(ptrCondCfg);

            return gis_info_tmp;
        }

        #endregion

        public void Dispose()
        {
            foreach (DevLoginModel devLoginModel in _loginedDeviceDict.Values)
            {
                DeviceLogout(devLoginModel.LoginId);
            }

            CHCNetSDK.NET_DVR_Cleanup();
        }

        #endregion
    }
}
