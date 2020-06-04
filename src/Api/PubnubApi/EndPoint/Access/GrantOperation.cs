﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PubnubApi.Interface;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Globalization;

namespace PubnubApi.EndPoint
{
    public class GrantOperation : PubnubCoreBase
    {
        private readonly PNConfiguration config;
        private readonly IJsonPluggableLibrary jsonLibrary;
        private readonly IPubnubUnitTest unit;
        private readonly IPubnubLog pubnubLog;
        private readonly EndPoint.TelemetryManager pubnubTelemetryMgr;

        private string[] pubnubChannelNames;
        private string[] pubnubChannelGroupNames;
        private string[] pamAuthenticationKeys;
        private bool grantWrite;
        private bool grantRead;
        private bool grantManage;
        private bool grantDelete;
        private long grantTTL = -1;
        private PNCallback<PNAccessManagerGrantResult> savedCallback;
        private Dictionary<string, object> queryParam;

        public GrantOperation(PNConfiguration pubnubConfig, IJsonPluggableLibrary jsonPluggableLibrary, IPubnubUnitTest pubnubUnit, IPubnubLog log, EndPoint.TelemetryManager telemetryManager, Pubnub instance) : base(pubnubConfig, jsonPluggableLibrary, pubnubUnit, log, telemetryManager, instance)
        {
            config = pubnubConfig;
            jsonLibrary = jsonPluggableLibrary;
            unit = pubnubUnit;
            pubnubLog = log;
            pubnubTelemetryMgr = telemetryManager;
        }

        public GrantOperation Channels(string[] channels)
        {
            this.pubnubChannelNames = channels;
            return this;
        }

        public GrantOperation ChannelGroups(string[] channelGroups)
        {
            this.pubnubChannelGroupNames = channelGroups;
            return this;
        }

        public GrantOperation AuthKeys(string[] authKeys)
        {
            this.pamAuthenticationKeys = authKeys;
            return this;
        }

        public GrantOperation Write(bool write)
        {
            this.grantWrite = write;
            return this;
        }

        public GrantOperation Read(bool read)
        {
            this.grantRead = read;
            return this;
        }

        public GrantOperation Manage(bool manage)
        {
            this.grantManage = manage;
            return this;
        }

        public GrantOperation Delete(bool delete)
        {
            this.grantDelete = delete;
            return this;
        }

        public GrantOperation TTL(long ttl)
        {
            this.grantTTL = ttl;
            return this;
        }

        public GrantOperation QueryParam(Dictionary<string, object> customQueryParam)
        {
            this.queryParam = customQueryParam;
            return this;
        }

        [Obsolete("Async is deprecated, please use Execute instead.")]
        public void Async(PNCallback<PNAccessManagerGrantResult> callback)
        {
            Execute(callback);
        }

        public void Execute(PNCallback<PNAccessManagerGrantResult> callback)
        {
            if (config != null && pubnubLog != null)
            {
                LoggingMethod.WriteToLog(pubnubLog, string.Format("DateTime: {0}, WARNING: Grant() signature has changed! This specific call will be making a request to PAMv2. Please update your code if this is not the intended action.", DateTime.Now.ToString(CultureInfo.InvariantCulture)), config.LogVerbosity);
            }
#if NETFX_CORE || WINDOWS_UWP || UAP || NETSTANDARD10 || NETSTANDARD11 || NETSTANDARD12
            Task.Factory.StartNew(() =>
            {
                this.savedCallback = callback;
                GrantAccess(this.pubnubChannelNames, this.pubnubChannelGroupNames, this.pamAuthenticationKeys, this.grantRead, this.grantWrite, this.grantDelete, this.grantManage, this.grantTTL, this.queryParam, callback);
            }, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default).ConfigureAwait(false);
#else
            new Thread(() =>
            {
                this.savedCallback = callback;
                GrantAccess(this.pubnubChannelNames, this.pubnubChannelGroupNames, this.pamAuthenticationKeys, this.grantRead, this.grantWrite, this.grantDelete, this.grantManage, this.grantTTL, this.queryParam, callback);
            })
            { IsBackground = true }.Start();
#endif
        }

        public async Task<PNResult<PNAccessManagerGrantResult>> ExecuteAsync()
        {
            if (config != null && pubnubLog != null)
            {
                LoggingMethod.WriteToLog(pubnubLog, string.Format("DateTime: {0}, WARNING: Grant() signature has changed! This specific call will be making a request to PAMv2. Please update your code if this is not the intended action.", DateTime.Now.ToString(CultureInfo.InvariantCulture)), config.LogVerbosity);
            }
            return await GrantAccess(this.pubnubChannelNames, this.pubnubChannelGroupNames, this.pamAuthenticationKeys, this.grantRead, this.grantWrite, this.grantDelete, this.grantManage, this.grantTTL, this.queryParam).ConfigureAwait(false);
        }

        internal void Retry()
        {
#if NETFX_CORE || WINDOWS_UWP || UAP || NETSTANDARD10 || NETSTANDARD11 || NETSTANDARD12
            Task.Factory.StartNew(() =>
            {
                GrantAccess(this.pubnubChannelNames, this.pubnubChannelGroupNames, this.pamAuthenticationKeys, this.grantRead, this.grantWrite, this.grantDelete, this.grantManage, this.grantTTL, this.queryParam, savedCallback);
            }, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default).ConfigureAwait(false);
#else
            new Thread(() =>
            {
                GrantAccess(this.pubnubChannelNames, this.pubnubChannelGroupNames, this.pamAuthenticationKeys, this.grantRead, this.grantWrite, this.grantDelete, this.grantManage, this.grantTTL, this.queryParam, savedCallback);
            })
            { IsBackground = true }.Start();
#endif
        }

        internal void GrantAccess(string[] channels, string[] channelGroups, string[] authKeys, bool read, bool write, bool delete, bool manage, long ttl, Dictionary<string, object> externalQueryParam, PNCallback<PNAccessManagerGrantResult> callback)
        {
            if (string.IsNullOrEmpty(config.SecretKey) || string.IsNullOrEmpty(config.SecretKey.Trim()) || config.SecretKey.Length <= 0)
            {
                throw new MissingMemberException("Invalid secret key");
            }

            List<string> channelList = new List<string>();
            List<string> channelGroupList = new List<string>();
            List<string> authList = new List<string>();

            if (channels != null && channels.Length > 0)
            {
                channelList = new List<string>(channels);
                channelList = channelList.Where(ch => !string.IsNullOrEmpty(ch) && ch.Trim().Length > 0).Distinct<string>().ToList();
            }

            if (channelGroups != null && channelGroups.Length > 0)
            {
                channelGroupList = new List<string>(channelGroups);
                channelGroupList = channelGroupList.Where(cg => !string.IsNullOrEmpty(cg) && cg.Trim().Length > 0).Distinct<string>().ToList();
            }

            if (authKeys != null && authKeys.Length > 0)
            {
                authList = new List<string>(authKeys);
                authList = authList.Where(auth => !string.IsNullOrEmpty(auth) && auth.Trim().Length > 0).Distinct<string>().ToList();
            }

            string channelsCommaDelimited = string.Join(",", channelList.ToArray().OrderBy(x => x).ToArray());
            string channelGroupsCommaDelimited = string.Join(",", channelGroupList.ToArray().OrderBy(x => x).ToArray());
            string authKeysCommaDelimited = string.Join(",", authList.ToArray().OrderBy(x => x).ToArray());

            IUrlRequestBuilder urlBuilder = new UrlRequestBuilder(config, jsonLibrary, unit, pubnubLog, pubnubTelemetryMgr, (PubnubInstance != null) ? PubnubInstance.InstanceId : "");
            Uri request = urlBuilder.BuildGrantV2AccessRequest("GET", "", channelsCommaDelimited, channelGroupsCommaDelimited, authKeysCommaDelimited, read, write, delete, manage, ttl, externalQueryParam);

            RequestState<PNAccessManagerGrantResult> requestState = new RequestState<PNAccessManagerGrantResult>();
            requestState.Channels = channels;
            requestState.ChannelGroups = channelGroups;
            requestState.ResponseType = PNOperationType.PNAccessManagerGrant;
            requestState.PubnubCallback = callback;
            requestState.Reconnect = false;
            requestState.EndPointOperation = this;

            UrlProcessRequest(request, requestState, false).ContinueWith(r =>
            {
                string json = r.Result.Item1;
                if (!string.IsNullOrEmpty(json))
                {
                    List<object> result = ProcessJsonResponse(requestState, json);
                    ProcessResponseCallbacks(result, requestState);
                }
            }, TaskContinuationOptions.ExecuteSynchronously).Wait();
            
        }

        internal async Task<PNResult<PNAccessManagerGrantResult>> GrantAccess(string[] channels, string[] channelGroups, string[] authKeys, bool read, bool write, bool delete, bool manage, long ttl, Dictionary<string, object> externalQueryParam)
        {
            if (string.IsNullOrEmpty(config.SecretKey) || string.IsNullOrEmpty(config.SecretKey.Trim()) || config.SecretKey.Length <= 0)
            {
                throw new MissingMemberException("Invalid secret key");
            }

            PNResult<PNAccessManagerGrantResult> ret = new PNResult<PNAccessManagerGrantResult>();

            List<string> channelList = new List<string>();
            List<string> channelGroupList = new List<string>();
            List<string> authList = new List<string>();

            if (channels != null && channels.Length > 0)
            {
                channelList = new List<string>(channels);
                channelList = channelList.Where(ch => !string.IsNullOrEmpty(ch) && ch.Trim().Length > 0).Distinct<string>().ToList();
            }

            if (channelGroups != null && channelGroups.Length > 0)
            {
                channelGroupList = new List<string>(channelGroups);
                channelGroupList = channelGroupList.Where(cg => !string.IsNullOrEmpty(cg) && cg.Trim().Length > 0).Distinct<string>().ToList();
            }

            if (authKeys != null && authKeys.Length > 0)
            {
                authList = new List<string>(authKeys);
                authList = authList.Where(auth => !string.IsNullOrEmpty(auth) && auth.Trim().Length > 0).Distinct<string>().ToList();
            }

            string channelsCommaDelimited = string.Join(",", channelList.ToArray().OrderBy(x => x).ToArray());
            string channelGroupsCommaDelimited = string.Join(",", channelGroupList.ToArray().OrderBy(x => x).ToArray());
            string authKeysCommaDelimited = string.Join(",", authList.ToArray().OrderBy(x => x).ToArray());

            IUrlRequestBuilder urlBuilder = new UrlRequestBuilder(config, jsonLibrary, unit, pubnubLog, pubnubTelemetryMgr, (PubnubInstance != null) ? PubnubInstance.InstanceId : "");
            Uri request = urlBuilder.BuildGrantV2AccessRequest("GET", "", channelsCommaDelimited, channelGroupsCommaDelimited, authKeysCommaDelimited, read, write, delete, manage, ttl, externalQueryParam);

            RequestState<PNAccessManagerGrantResult> requestState = new RequestState<PNAccessManagerGrantResult>();
            requestState.Channels = channels;
            requestState.ChannelGroups = channelGroups;
            requestState.ResponseType = PNOperationType.PNAccessManagerGrant;
            requestState.Reconnect = false;
            requestState.EndPointOperation = this;

            Tuple<string, PNStatus> JsonAndStatusTuple = await UrlProcessRequest(request, requestState, false).ConfigureAwait(false);
            ret.Status = JsonAndStatusTuple.Item2;
            string json = JsonAndStatusTuple.Item1;
            if (!string.IsNullOrEmpty(json))
            {
                List<object> resultList = ProcessJsonResponse(requestState, json);
                if (resultList != null && resultList.Count > 0)
                {
                    ResponseBuilder responseBuilder = new ResponseBuilder(config, jsonLibrary, pubnubLog);
                    PNAccessManagerGrantResult responseResult = responseBuilder.JsonToObject<PNAccessManagerGrantResult>(resultList, true);
                    if (responseResult != null)
                    {
                        ret.Result = responseResult;
                    }
                }
            }
            
            return ret;
        }

        internal void CurrentPubnubInstance(Pubnub instance)
        {
            PubnubInstance = instance;

            if (!ChannelRequest.ContainsKey(instance.InstanceId))
            {
                ChannelRequest.GetOrAdd(instance.InstanceId, new ConcurrentDictionary<string, HttpWebRequest>());
            }
            if (!ChannelInternetStatus.ContainsKey(instance.InstanceId))
            {
                ChannelInternetStatus.GetOrAdd(instance.InstanceId, new ConcurrentDictionary<string, bool>());
            }
            if (!ChannelGroupInternetStatus.ContainsKey(instance.InstanceId))
            {
                ChannelGroupInternetStatus.GetOrAdd(instance.InstanceId, new ConcurrentDictionary<string, bool>());
            }
        }
    }
}
