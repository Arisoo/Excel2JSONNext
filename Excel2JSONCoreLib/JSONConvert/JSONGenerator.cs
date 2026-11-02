using Excel2JSONCoreLib.ViewModels;
using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2JSONCoreLib.JSONConvert
{    
    public class JSONGenerator
    {
        /// <summary>
        /// 엑셀파일 1개를 1개의 JSON 으로 컨버팅
        ///     SomeExcel.xlsx -> class SomeDatatable ; 물론 이 코드는 Serialize 수동 코드
        ///         seet1 ->  SomeDatatable { public List<???> seet1; }
        /// </summary>
        /// <param name="filefullpath">excel 파일 하나의 풀경로</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static string ExcelToJSONV4(string filefullpath, MainWindowViewModel logger)
        {
            Dictionary<string, List<Dictionary<string, object>>> objectmap = new Dictionary<string, List<Dictionary<string, object>>>();

            logger.AppendLogStringLine($"{DateTime.Now.ToString()} - {filefullpath} Start...");
                        
            using (var stream = File.Open(filefullpath, FileMode.Open, FileAccess.Read))
            {                
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
             

                    // seet 단위로 진행한다.
                    //  seet가 하나의 List 컨테이너가 된다.
                    do
                    {
                        #region 시트이름 규칙 검사
                        string seetname = string.Empty;
                        if (Utils.IsFilteredSeet(reader.Name))
                        {
                            //reader.NextResult();
                            continue; // 다음 시트로 감
                        }
                        seetname = reader.Name;

                        // 변수명 규칙으로 조사할 필요가 있다.
                        if (false == Utils.IsValidIdentifier(reader.Name))
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} passed 변수명 규칙에 안맞음");
                            continue; // 다음 시트로 감
                        }
                        #endregion 시트이름 규칙 검사

                        #region line-1-Fields Define
                        
                        // A1 셀이 비어있으면 bypass 로 관주
                        var b = reader.Read();
                        if (false == b || string.IsNullOrEmpty(reader.GetString(0)))
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} passed A1 셀 비어 있음");
                            continue; // 다음 시트로 감
                        }

                        List<FieldMap> fields = new List<FieldMap>();                        	
                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            // 비어 있는 칸을 만나면 End Of Field/Column 으로 관주
                            //  그래서 참고및 관리용 필드등을 만들고자하면 실제 쓰는 필드들로 부터 우측으로 필드를 띄워서 분리하여 작성하면 된다.
                            if (string.IsNullOrEmpty(reader.GetString(i))) break;

                            var fn = reader.GetString(i).Trim();

                            if (false == Utils.IsValidIdentifier(fn))
                            {
                                logger.AppendLogStringLine($"\tfield {fn} passed 변수명 규칙에 안맞음");
                                continue;
                            }
                            FieldMap fm = new FieldMap();
                            fm.fname = fn;
                            fm.index = i;
                            fields.Add(fm);
                        }

                        if (1 > fields.Count)
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} passed 컨버팅할 필드가 없음");
                            continue; // 다음 시트로 감
                        }

                        #endregion line-1-Fields Define

                        #region line 2-3-4 exists check
                        // 2-3-4 라인도 유효해야 한다. 실제로 이 라인들을 통해 class gen 을 하기에
                        //  2: 자료형
                        //  3: 기본값 할당을 위해 예약해둔 필드, 우선은 NONE 등 아무거나, null-or-empty 만 아니면 됨
                        //  4: 필드에 대한 설명/주석 관리 목적이지만, 추후 이걸 class gen 할때 필드의 주석으로..., null-or-empty 만 아니면 됨
                        
                        b = reader.Read(); // 2
                        if (false == b)
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} 2번째 라인이 없음 (필드 자료형)");
                            continue; // 다음 시트로 감
                        }
                        else
                        {
                            for (int i = 0; i < fields.Count; i++)
                            {
                                var fm = fields[i];
                                fm.typename = reader.GetValue(fm.index).ToString();
                            }
                        }
                        b = reader.Read(); // 3
                        if (false == b)
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} 3번째 라인이 없음 (미사용이지만 아무 값이라도)");
                            continue; // 다음 시트로 감
                        }
                        b = reader.Read(); // 4
                        if (false == b)
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} 4번째 라인이 없음 (관리목적 주석,설명이지만 그래서 더욱더");
                            continue;
                        }
                        #endregion line 2-3-4 exists check

                        #region 실제 값 행들 처리 - 5번째 라인/줄/행 부터
                        List<Dictionary<string, object>> rowlist = new List<Dictionary<string, object>>();
                        fields = fields.OrderBy(row => row.index).ToList();
                        int ln = 4;
                        while (reader.Read())
                        {
                            // 라인 1개를 하나의 객체로 맵핑한다.

                            ++ln;
                            Dictionary<string, object> onerow = new Dictionary<string, object>();

                            // 첫번째 필드가 비어 있으면 EOS로 관주
                            var oc = reader.GetValue(fields[0].index);
                            if (null == oc || string.IsNullOrEmpty(oc.ToString()))
                            {
                                // 행의 첫 필드를 비워두면 End of File(Seet)로 관주
                                //  아직 반영안되면 안되는 행들은 실제 적용 행들로 부터 한행을 띄워서 관리하면 된다.
                                logger.AppendLogStringLine($"\tseet {ln} 라인 첫필드가 없어서 EndOfSeet 로 관주 끄읕~");
                                break;
                            }

                            // 필드 분석및 맵핑 시작!
                            foreach (var fm in fields)
                            {
                                // 필드의 값이 비어 있는(공백이 아니라) 경우 기본 값 할당
                                bool isnull = false;
                                var o = reader.GetValue(fm.index);
                                if (null == o)
                                {
                                    isnull = true;
                                    // 자료형의 이름도 typename(long) 이렇게 얻어내야 하지만, 안 바뀌니~
                                    switch (fm.typename?.ToLower())
                                    {
                                        case "long":
                                        case "int":
                                            o = 0;
                                            break;
                                        case "double":
                                        case "float":
                                            o = 0.0;
                                            break;
                                        case "string":
                                        case "stringkey":
                                            o = string.Empty;
                                            break;
                                        case "bool":
                                            o = false;
                                            break;
                                        case "datetime":
                                            o = default(DateTime);
                                            break;                                        
                                    }
                                    //continue;
                                }

                                if (!isnull)
                                {
                                    try
                                    {
                                        // 엑셀에서 숫자 가져올 때 무조건 double타입으로 인식되고 있음
                                        switch (fm.typename?.ToLower())
                                        {
                                            case "int":
                                                o = Convert.ToInt32(o);
                                                break;
                                            case "bool":
                                                o = Convert.ToDouble(o) > 0.0;
                                                break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.AppendLogStringLine($"==================================================================");
                                        logger.AppendLogStringLine($"ExcelInfo[ seet:{seetname} , type:{fm.typename} , field: {fm.fname} , value: {o}]");
                                        logger.AppendLogStringLine(ex.Message);
                                        logger.AppendLogStringLine(ex.StackTrace);
                                        return null;
                                    }
                                }
                                // nono trim!!
                                //var strv = o?.ToString();
                                //if (string.IsNullOrEmpty(strv))
                                //{
                                //    continue;
                                //}
                                onerow[fm.fname] = o;
                            }

                            // 필드갯수가 일치하지 않으면 추가하지 않는다.
                            if (onerow.Count < fields.Count)
                            {
                                logger.AppendLogStringLine($"\tseet {ln} 라인 필드갯수가 적어서 패스!");
                                break;
                            }

                            rowlist.Add(onerow);
                        }

                        #endregion 실제 값 행들 처리 - 5번째 라인/줄/행 부터

                        // 시트한개가 끝났다.
                        objectmap[seetname] = rowlist;

                        logger.AppendLogStringLine($"\t{seetname} count {rowlist.Count}");

                    } while (reader.NextResult());

                }
            }

            logger.AppendLogStringLine($"{DateTime.Now.ToString()} - Done");

            //return Newtonsoft.Json.JsonConvert.SerializeObject(objectmap, Newtonsoft.Json.Formatting.Indented);

            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                // 들여쓰기 활성화
                jsonWriter.Formatting = Formatting.Indented;

                // 들여쓰기 문자를 탭('\t')으로 설정
                jsonWriter.IndentChar = '\t';

                // 각 계층당 탭 개수 (기본값은 2이므로 1로 지정해야 탭 1개씩 들여씀)
                jsonWriter.Indentation = 1;

                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, objectmap);

                return stringWriter.ToString();
            }
        }
    }
}
