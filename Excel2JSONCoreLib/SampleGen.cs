using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace Common.DataTables
{
    public partial class DT_InheritanceFactor
    {

        public partial class primes_Node
        {
            // 고유키
            [JsonProperty]
            public readonly int keyidx;

            // 이름키
            [JsonProperty]
            public readonly string namekey;

            // 백패널이미지 스프라이트 이름
            [JsonProperty]
            public readonly string bgimg_large;

            // 백패널이미지 스프라이트 이름
            [JsonProperty]
            public readonly string bgimg_mid;

            // 백패널이미지 스프라이트 이름
            [JsonProperty]
            public readonly string bgimg_small;

            // 아이콘 스프라이트 이름
            [JsonProperty]
            public readonly string icon;

            // 계승 받았을때 처리 내용
            [JsonProperty]
            public readonly string activeproc;

            // 물려주기 위한 취득 조건
            [JsonProperty]
            public readonly string gathercondition;

            // 별갯수
            [JsonProperty]
            public readonly int grade;

            // 업글가능시 다음키
            [JsonProperty]
            public readonly int upnext;

        }


        public List<primes_Node> primes;
    }
}

