/// <summary>
/// Summary description for QuestionOptions



public class QuestionOptions
{
    public int Id { get; set; }
    public string QuestionsIds { get; set; }
    public int opt { get; set; }
    public int QID { get; set; }
    public string OptionLabel { get; set; }
    public string OptionCode { get; set; }
    public int ProjectID { get; set; }
    public int CountryID { get; set; }
    public int Terminate { get; set; }
    public int Logic { get; set; }
    public int Quota { get; set; }
}
public class SurveyResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int QuestionId { get; set; }
    public string UID { get; set; }
    public string Options { get; set; }
    public string QID { get; set; }
    public string QLabel { get; set; }
    public int QType { get; set; }
    public string OptionLabel { get; set; }
    public string OptionCode { get; set; }
}
public class ResposeDataDownload
{
    public int Id { get; set; }
    public string UID { get; set; }
    public string Col1 { get; set; }
    public string Col2 { get; set; }
    public string Col3 { get; set; }
    public string Col4 { get; set; }
    public string Col5 { get; set; }
    public string Col6 { get; set; }
    public string Col7 { get; set; }
    public string Col8 { get; set; }
    public string Col9 { get; set; }
    public string Col10 { get; set; }
    public string Col11 { get; set; }
    public string Col12 { get; set; }
    public string Col13 { get; set; }
    public string Col14 { get; set; }
    public string Col15 { get; set; }
    public string Col16 { get; set; }
    public string Col17 { get; set; }
    public string Col18 { get; set; }
    public string Col19 { get; set; }
    public string Col20 { get; set; }
    public string Col21 { get; set; }
    public string Col22 { get; set; }
    public string Col23 { get; set; }
    public string Col24 { get; set; }
    public string Col25 { get; set; }
    public string Col26 { get; set; }
    public string Col27 { get; set; }
    public string Col28 { get; set; }
    public string Col29 { get; set; }
    public string Col30 { get; set; }
    public string Col31 { get; set; }
    public string Col32 { get; set; }
    public string Col33 { get; set; }
    public string Col34 { get; set; }
    public string Col35 { get; set; }
    public string Col36 { get; set; }
    public string Col37 { get; set; }
    public string Col38 { get; set; }
    public string Col39 { get; set; }
    public string Col40 { get; set; }
    public string Col41 { get; set; }
    public string Col42 { get; set; }
    public string Col43 { get; set; }
    public string Col44 { get; set; }
    public string Col45 { get; set; }
    public string Col46 { get; set; }
    public string Col47 { get; set; }
    public string Col48 { get; set; }
    public string Col49 { get; set; }
    public string Col50 { get; set; }
    public string Col51 { get; set; }
    public string Col52 { get; set; }
    public string Col53 { get; set; }
    public string Col54 { get; set; }
    public string Col55 { get; set; }
    public string Col56 { get; set; }
    public string Col57 { get; set; }
    public string Col58 { get; set; }
    public string Col59 { get; set; }
    public string Col60 { get; set; }
}
public class OptionMapping
{
    public int Id { get; set; }
    public int OptionId { get; set; }
    public int PID { get; set; }
    public int CID { get; set; }
    public int QID { get; set; }
    public int Logic { get; set; }
    public int Quota { get; set; }
    public int Active { get; set; }
    public int opt { get; set; }
}