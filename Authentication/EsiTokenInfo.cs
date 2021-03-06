﻿using System;
using EntrepreneurCommon.Client;
using EntrepreneurCommon.Models.Esi;
using Newtonsoft.Json;
using Nito.AsyncEx;
using RestSharp;

namespace EntrepreneurCommon.Authentication
{
    /// <summary>
    /// Custom container class for an ESI token which composites the Access+Refresh Tokens with the Verification response for easy access to extra information. Also includes self-contained methods for retrieving additional, basic information, subject to personal definition.
    /// </summary>
    /// TODO: Rename the class to something more intuitive, like EsiTokenContainer or EsiTokenWrapper?
    [JsonObject(MemberSerialization.OptIn)]
    public partial class EsiTokenInfo : IEsiTokenContainer
    {
        // Main fields
        [JsonProperty("Verificationinfo")] private IEsiTokenVerification tokenVerification;
        [JsonProperty("TokenInfo")] private IEsiTokenResponse tokenAccessInfo;
        [JsonProperty("Enabled")] private bool _enabled = true;
        private CharacterPublicInformation characterInformation;
        private CharacterRolesModel characterRoles;

        /// <summary>
        /// Reference to the EsiAuthClient that was responsible for generating/refreshing the token. Assigned automatically when generating the token from the AuthClient, but needs to be manually re-assigned when loading the object from DataStore (i.e: when restarting the app and loading saved tokens).
        /// </summary>
        public EsiAuthClient AuthClient { get; set; }

        //public EsiTokenVerification TokenVerification { get; set; }
        //public EsiTokenResponse TokenAccessInfo { get; set; }

        public Boolean Enabled {
            get => _enabled;
            set => _enabled = value;
        }

    #region Forward TokenVerification Info

        public int CharacterId {
            get => tokenVerification.CharacterId;
            set => tokenVerification.CharacterId = value;
        }

        public string CharacterName {
            get => tokenVerification.CharacterName;
            set => tokenVerification.CharacterName = value;
        }

        public string CharacterOwnerHash {
            get => tokenVerification.CharacterOwnerHash;
            set => tokenVerification.CharacterOwnerHash = value;
        }

        public string ExpiresOn {
            get => Expiry;
            set => Expiry = value;
        }

        public string Expiry {
            get => tokenVerification.ExpiresOn;
            set => tokenVerification.ExpiresOn = value;
        }

        public string Scopes {
            get => tokenVerification.Scopes;
            set => tokenVerification.Scopes = value;
        }

        public string TokenTypeVerification {
            get => tokenVerification.TokenType;
        }

    #endregion

    #region Forward TokenResponse info

        public string RefreshToken {
            get => tokenAccessInfo.RefreshToken;
            set => tokenAccessInfo.RefreshToken = value;
        }

        public string AccessToken {
            get => tokenAccessInfo.AccessToken;
            set => tokenAccessInfo.AccessToken = value;
        }

        public string TokenType {
            get => tokenAccessInfo.TokenType;
            set => tokenAccessInfo.TokenType = value;
        }

        public Int32 ExpiresIn {
            get => tokenAccessInfo.ExpiresIn;
            set => tokenAccessInfo.ExpiresIn = value;
        }

    #endregion

        // Automatic token refreshing
        public string AccessTokenAuto {
            get => AsyncContext.Run(GetToken);
        }

        // Forward Character Info
        public Int32 CorporationId {
            get => CharacterInformation.CorporationID;
        }

        public Int32 AllianceId {
            get => CharacterInformation.AllianceID;
        }

        // Requestable info
        private IRestResponse<CharacterPublicInformation> characterInformationResponse { get; set; }
        private IRestResponse<CharacterRolesModel> characterRolesResponse { get; set; }

        [JsonProperty("CharacterInformation")]
        public CharacterPublicInformation CharacterInformation {
            get => GetCharacterInformation();
        }

        [JsonProperty("CharacterRoles")]
        public CharacterRolesModel CharacterRoles {
            get => GetCharacterRoles();
        }

        // Other
        public Action OnTokenUpdated;

        // Additional application-managable info
        public string CorporationName { get; set; }
        public string AllianceName { get; set; }

        public void OverrideTokenData(EsiTokenVerification tokenVerification, EsiTokenResponse tokenAccessInfo)
        {
            this.tokenVerification = tokenVerification;
            this.tokenAccessInfo = tokenAccessInfo;
        }
    }
}