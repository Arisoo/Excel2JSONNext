using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2JSONCoreLib
{
    //public enum SystemStringEnums
    //{
    //    SEETNAME_ERROR = 1,
    //    A1_CELL_EMPTY,
    //    FIELDNAME_ERROR,
    //    FIELD_ZERO_COUNT,
    //    FIFTH_LINE_NOEXIST,
    //    REACH_EOF_FIRSTFIELD_EMPTY,
    //    REACH_EOF_LESS_FIELD,
    //    INFO_SEETEND,
    //    INFO_FILE_DONE,
    //}

    public partial class SystemString
    {
        public static string INFO_FILE_START = "{0} Start";
        public static string INFO_SEET_START = "\tseet {0} Start";
        public static string INFO_SEET_EXCEPT = "\tseet 제외 시트";
        public static string SEETNAME_ERROR = "\tseet {0} 건너띔 변수명 규칙에 안맞음";
        public static string A1_CELL_EMPTY = "\tseet {0} 건너띔 A1 셀 비어 있음";
        public static string FIELDNAME_ERROR = "\tfield {0} field {1} name - 건너띔 변수명 규칙에 안맞거나 비어있음";
        public static string FIELDNAME_CHOOSE = "\tfield {0} field {1} 필드 선택";
        public static string FIELD_ZERO_COUNT = "\tseet {0} 건너띔 컨버팅할 필드가 없음";
        public static string TYPE_LINE_NOEXIST = "\tseet {0} 2번째 라인이 없음 (필드 자료형)";
        public static string DV_LINE_NOEXIST = "\tseet {0} 3번째 라인이 없음 (미사용이지만 아무 값이라도)";
        public static string COMMENT_LINE_NOEXIST = "\tseet {0} 4번째 라인이 없음 (관리목적 주석,설명이지만 그래서 더욱더";
        public static string REACH_EOF_FIRSTFIELD_EMPTY = "\tseet {0} 라인 첫필드가 없어서 EOF로 관주 끄읕~";
        public static string REACH_EOF_LESS_FIELD = "\tseet {0} 라인 필드갯수가 적어서 패스!";
        public static string INFO_SEETEND = "\t{0} count {1}";
        public static string INFO_FILE_DONE = "{0} - Done";
    }
}
