﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Hurricane.Music.Track.WebApi.VkontakteApi
{
    public class VkAuthorization
    {
        private readonly List<NameValue> _decodedAnswer;

        private VkAuthorization(Uri responseUrl)
        {
            _decodedAnswer = Decode(responseUrl);
        }

        public static VkAuthorization From(Uri responseUrl)
        {
            return new VkAuthorization(responseUrl);
        }

        public bool IsAuthorized
        {
            get { return AccessToken != null; }
        }

        public bool IsAuthorizationRequired
        {
            get { return GetFieldValue("__q_hash") != null; }
        }

        public string AccessToken
        {
            get { return GetFieldValue("access_token"); }
        }

        public string ExpiresIn
        {
            get { return GetFieldValue("expires_in"); }
        }

        public long UserId
        {
            get
            {
                var userIdFieldValue = GetFieldValue("user_id");
                long userId;
                if (!long.TryParse(userIdFieldValue, out userId))
                    throw new Exception("UserId is not integer value.");

                return userId;
            }
        }

        public long? CaptchaId
        {
            get
            {
                var sidFieldValue = GetFieldValue("sid");
                long sid;
                if (long.TryParse(sidFieldValue, out sid))
                    return sid;

                return null;
            }
        }

        private string GetFieldValue(string fieldName)
        {
            return _decodedAnswer.Where(i => i.Name == fieldName).Select(i => i.Value).FirstOrDefault();
        }

        internal sealed class NameValue
        {
            public string Name { get; set; }

            public string Value { get; set; }

            public NameValue(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public override string ToString()
            {
                return string.Format("{0}={1}", Name, Value);
            }
        }

        private static List<NameValue> Decode(Uri url)
        {
            if (!string.IsNullOrEmpty(url.Query))
                return DecodeQuery(url);

            return DecodeFragment(url);
        }

        private static List<NameValue> DecodeQuery(Uri url)
        {
            var urlQuery = url.Query;
            var query = urlQuery.StartsWith("?") || urlQuery.StartsWith("#") ? urlQuery.Substring(1) : urlQuery;
            return query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Split('=')).Select(s => new NameValue(s[0], s[1])).ToList();
        }

        private static List<NameValue> DecodeFragment(Uri url)
        {
            var urlQuery = url.Fragment;
            var query = urlQuery.StartsWith("#") ? urlQuery.Substring(1) : urlQuery;
            return query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Split('=')).Select(s => new NameValue(s[0], s[1])).ToList();
        }
    }
}
