using Excel2JSONCoreLib.ViewModels;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2JSONCoreLib.ClassGenerator
{
    public class ClassCSGenerator
    {
        /// <summary>
        /// filefullpath 엑셀파일 하나에 대응되는 cs 클래스 파일을 생성
        ///     
        /// </summary>
        /// <param name="filefullpath"></param>
        /// <param name="SeetNodeBaseClassName"></param>
        /// <param name="clsname"></param>
        /// <param name="ns"></param>
        /// <param name="logger"></param>
        public static string GenCS(string filefullpath, 
            string ContainerBaseClassName, string SeetNodeBaseClassName, string clsname, string ns,
            MainWindowViewModel logger)
        {
            logger.AppendLogStringLine($"{DateTime.Now.ToString()} - {filefullpath} Start...");
            // 1시트는 1개의 List 변수로 변환됨.            
            Dictionary<string, string> listVariables = new ();
            List<StringBuilder> clsdatas = new ();
            using (var stream = File.Open(filefullpath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    
                    do
                    {
                        string seetname = reader.Name;

                        logger.AppendLogStringLine($"seet {seetname} class gen 시작!");

                        // 아래에 해당되면 건너띄기
                        if (Utils.IsFilteredSeet(reader.Name))
                        {
                            //reader.NextResult();
                            logger.AppendLogStringLine($"\tseet {seetname} passed 이름 필터링");
                            continue; // 다음 seet 로~
                        }

                        // 변수명 규칙으로 조사할 필요가 있다.
                        if (false == Utils.IsValidIdentifier(reader.Name))
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} passed 변수명 규칙에 안맞음");
                            continue; // 다음 seet 로~
                        }

                        string seetclsname = $"{seetname}_Node";

                        var b = reader.Read();
                        if (false == b || string.IsNullOrEmpty(reader.GetString(0)))
                        {
                            //reader.NextResult();
                            logger.AppendLogStringLine($"\tseet {seetname} passed A1 셀 비어 있음");
                            continue; // 다음 seet 로~
                        }

                        
                        List<FieldMap> fields = new List<FieldMap>();

                        #region 첫행은 필드명						
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
                        #endregion 첫행은 필드명						

                        #region 2번째행은 자료형
                        b = reader.Read();
                        if (false == b)
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} passed 필드타입 정의 행이 비어 있음");
                            continue; // 다음 seet 로~
                        }
                        foreach (var fm in fields)
                        {
                            var o = reader.GetValue(fm.index);
                            if (null == o)
                            {
                                continue;
                            }
                            var strv = o.ToString().Trim();
                            if (string.IsNullOrEmpty(strv))
                            {
                                continue;
                            }
                            fm.typename = strv;
                            fm.bCompleted = true;
                        }
                        #endregion 2번째행은 자료형

                        #region 3번째행은 안쓰지만 존재해야함!
                        b = reader.Read(); // 3
                        if (false == b)
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} 3번째 라인이 없음 (미사용이지만 아무 값이라도)");
                            continue; // 다음 시트로 감
                        }
                        #endregion 3번째행은 안쓰지만 존재해야함!

                        #region 4번째행은 필드 설명
                        b = reader.Read(); // 4
                        if (false == b)
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} 4번째 라인이 없음 (관리목적 주석,설명이지만 그래서 더욱더");
                            continue;
                        }
                        foreach (var fm in fields)
                        {
                            var o = reader.GetValue(fm.index);
                            if (null == o)
                            {
                                continue;
                            }
                            var strv = o.ToString().Trim();
                            if (string.IsNullOrEmpty(strv))
                            {
                                continue;
                            }
                            fm.comment = strv;
                        }
                        #endregion 4번째행은 필드 설명

                        if (1 > fields.Count(row => row.bCompleted == true))
                        {
                            logger.AppendLogStringLine($"\tseet {seetname} passed 필드 자료형이 1건도 없음");
                            continue; // 다음 seet 로~
                        }

                        #region Seet 대응 클래스 생성

                        StringBuilder sb = new StringBuilder();

                        sb.Append($"\t\tpublic partial class {seetclsname}");
                        if (!string.IsNullOrEmpty(SeetNodeBaseClassName)) sb.Append($" : {SeetNodeBaseClassName}");
                        sb.Append(Environment.NewLine);

                        sb.AppendLine("\t\t{");

                        foreach (var fm in fields)
                        {
                            // fm.typename 이 none 이면 주석 처리를 해두자
                            bool isNone = fm.typename.ToLower() == "none";
                            if(isNone)
                            {
                                sb.AppendLine(string.Format($"\t\t\t//public {fm.typename} {fm.fname};"));
                                sb.AppendLine();
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(fm.comment))
                                {
                                    sb.AppendLine(string.Format($"\t\t\t// {fm.comment}"));
                                }
                                sb.AppendLine("\t\t\t[JsonProperty]");
                                sb.AppendLine(string.Format($"\t\t\tpublic readonly {fm.typename} {fm.fname};"));
                                sb.AppendLine();
                            }
                        }


                        sb.AppendLine("\t\t}");
                        clsdatas.Add(sb);
                        #endregion Seet 대응 클래스 생성

                        // seet 대응 컨테이너 변수 지정
                        listVariables[seetname] = $"public List<{seetclsname}> {seetname};";

                        logger.AppendLogStringLine($"\tseet {seetname} gen done");

                    } while (reader.NextResult());

                }
            }

            StringBuilder osb = new StringBuilder();
            osb.AppendLine($"using Newtonsoft.Json");
            osb.AppendLine($"using System;");
            osb.AppendLine($"using System.Collections.Generic;");


            osb.AppendLine($"namespace {ns}");
            osb.AppendLine("{");


            osb.Append($"\tpublic partial class {clsname}");
            if (!string.IsNullOrEmpty(ContainerBaseClassName)) osb.Append($" : {ContainerBaseClassName}");
            osb.Append(Environment.NewLine);
            osb.AppendLine("\t{");

            foreach (var cls in clsdatas)
            {
                osb.AppendLine();
                osb.AppendLine(cls.ToString());
                osb.AppendLine();
            }

            //dtcls[seetname] = $"List<{seetclsname}> {seetname}";
            foreach (var kp in listVariables)
            {
                osb.AppendLine("\t\t" + kp.Value);
            }

            osb.AppendLine("\t}");

            osb.AppendLine("}");

            logger.AppendLogStringLine($"{DateTime.Now.ToString()} - Done");

            //File.WriteAllText(outfilepath, osb.ToString());
            return osb.ToString();
        }
    }
}
