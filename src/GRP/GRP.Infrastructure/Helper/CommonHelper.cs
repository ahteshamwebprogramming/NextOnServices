using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using GRP.Infrastructure.Models.Account;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;

namespace GRP.Infrastructure.Helper;

public static class CommonHelper
{
    private static Random random = new Random();

    public static T GetClassObject<T>(T t) where T : class, new()
    {
        if (t == null)
        {
            t = new T();
        }
        return t;
    }
    public static string RandomString()
    {
        try
        {
            string s = Guid.NewGuid().ToString("N").ToLower()
                  .Replace("1", "").Replace("o", "").Replace("0", "")
                  .Substring(0, 10);
            return s;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static bool IsNumeric(string Value)
    {
        return decimal.TryParse(Value, out _) || double.TryParse(Value, out _);

    }
    public static string NewLineEntry()
    {
        return "<br>";
    }
    public static DateTime? ConvertToDateTime(string inputDateTime, string format)
    {
        //dd/MM/yyyy
        try
        {
            DateTime? dateTime;
            if (inputDateTime != null && inputDateTime.Trim() != "")
            {
                IFormatProvider culture = new CultureInfo("en-US", true);
                dateTime = DateTime.ParseExact(inputDateTime, format, culture);
            }
            else
            {
                dateTime = null;
            }
            return dateTime;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public static string ConvertDatetimeToString(DateTime? input, string format)
    {
        try
        {
            if (input == null)
            {
                return "";
            }
            else
            {
                return (input ?? default(DateTime)).ToString(format);
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    public static string ConvertStringDateTimeToString(object input, string inputFormat, string outputFormat)
    {
        try
        {
            DateTime? datetime = ConvertToDateTime(input.ToString(), inputFormat);
            string outputdatetime = ConvertDatetimeToString(datetime, outputFormat);
            return outputdatetime;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public static DateTime? StringToDateTime(string sDateTime)
    {
        DateTime? returnDateTime = new DateTime();
        try
        {
            returnDateTime = DateTime.Parse(sDateTime);
            //        ,
            //"MM/dd/yyyy HH:mm:ss",
            //CultureInfo.InvariantCulture);
            return returnDateTime;
        }
        catch (Exception ex)
        {
            return returnDateTime;
        }
    }

    public static TimeOnly? StringToTimeOnly(string sTime)
    {
        TimeOnly? returnTime = new TimeOnly();
        try
        {
            returnTime = TimeOnly.ParseExact(sTime,
        "hh:mm:ss tt");
            return returnTime;
        }
        catch (Exception ex)
        {
            return returnTime;
        }
    }

    public static DateOnly? StringToDateOnly(string sDate)
    {
        DateOnly? returnDate = new DateOnly();
        try
        {
            returnDate = DateOnly.ParseExact(sDate,
        "MM/dd/yyyy");
            return returnDate;
        }
        catch (Exception ex)
        {
            return returnDate;
        }
    }
    public static HttpResponseMessage GetHttpResponseMessage<T>(T t, bool? isActive = true)
    {
        if (isActive == false)
            return ClientResponse.GetClientResponse(HttpStatusCode.Locked, "Inactive User");
        else if (t == null)
            return ClientResponse.GetClientResponse(HttpStatusCode.NotFound, "Wrong User Name and Password");
        else
            return ClientResponse.GetClientResponse(HttpStatusCode.OK, "Success");
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public static string Encrypt(string clearText)
    {
        string origninalText = clearText;
        string EncryptionKey = "MAKV2S54FUCKP99212";
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        if (clearText.Contains('/') || clearText.Contains('+'))
        {
            clearText = clearText.Replace("/", "slash").Replace("+", "plus");
        }
        return clearText;
    }
    public static string EncryptURL(string clearText)
    {
        clearText = Encrypt(clearText);
        clearText = WebUtility.UrlEncode(clearText);
        return clearText;
    }
    public static string EncryptURLHTML(string clearText)
    {
        clearText = Encrypt(clearText);
        //clearText = WebUtility.UrlEncode(clearText);
        //clearText = WebUtility.UrlEncode(clearText);
        //clearText = WebUtility.HtmlEncode(clearText);
        return clearText;
    }
    public static string DecryptURLHTML(string clearText)
    {
        //clearText = WebUtility.HtmlDecode(clearText);
        //clearText = WebUtility.UrlDecode(clearText);
        clearText = Decrypt(clearText);
        return clearText;
    }
    public static string DecryptURL(string clearText)
    {
        clearText = Decrypt(clearText);
        clearText = WebUtility.UrlDecode(clearText);
        return clearText;
    }
    public static string Decrypt(string cipherText)
    {
        cipherText = cipherText.Replace("slash", "/").Replace("plus", "+");
        string EncryptionKey = "MAKV2S54FUCKP99212";
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    //public static UserDTO GetEmployeeForSession(UserDTO UserDTORes)
    //{

    //    UserDTO UserDTO = new UserDTO();

    //    //UserDTO.EmployeeId = employeeMasterDTORes.EmployeeId;
    //    //employeeMasterDTO.EnycEmployeeId = employeeMasterDTORes.EnycEmployeeId;
    //    //employeeMasterDTO.ClientId = employeeMasterDTORes.ClientId;
    //    //employeeMasterDTO.UnitId = employeeMasterDTORes.UnitId;
    //    //employeeMasterDTO.EmployeeCode = employeeMasterDTORes.EmployeeCode;
    //    //employeeMasterDTO.FirstName = employeeMasterDTORes.FirstName;
    //    //employeeMasterDTO.MiddleName = employeeMasterDTORes.MiddleName;
    //    //employeeMasterDTO.LastName = employeeMasterDTORes.LastName;
    //    //employeeMasterDTO.EmployeeName = employeeMasterDTORes.EmployeeName;
    //    //employeeMasterDTO.Dob = employeeMasterDTORes.Dob;
    //    //employeeMasterDTO.GenderId = employeeMasterDTORes.GenderId;
    //    //employeeMasterDTO.Gender = employeeMasterDTORes.Gender;
    //    //employeeMasterDTO.FatherName = employeeMasterDTORes.FatherName;
    //    //employeeMasterDTO.ContactNo = employeeMasterDTORes.ContactNo;
    //    //employeeMasterDTO.EmailId = employeeMasterDTORes.EmailId;
    //    //employeeMasterDTO.Age = employeeMasterDTORes.Age;
    //    //employeeMasterDTO.SpouseName = employeeMasterDTORes.SpouseName;
    //    //employeeMasterDTO.ReligionId = employeeMasterDTORes.ReligionId;
    //    //employeeMasterDTO.MaritalStatusId = employeeMasterDTORes.MaritalStatusId;
    //    //employeeMasterDTO.AadharNumber = employeeMasterDTORes.AadharNumber;
    //    //employeeMasterDTO.Pannumber = employeeMasterDTORes.Pannumber;
    //    //employeeMasterDTO.BloodGroupId = employeeMasterDTORes.BloodGroupId;
    //    //employeeMasterDTO.ProfileImage = employeeMasterDTORes.ProfileImage;
    //    //employeeMasterDTO.ProfileImageFile = employeeMasterDTORes.ProfileImageFile;
    //    //employeeMasterDTO.Base64ProfileImage = employeeMasterDTORes.Base64ProfileImage;
    //    //employeeMasterDTO.WorkLocationId = employeeMasterDTORes.WorkLocationId;
    //    //employeeMasterDTO.Doj = employeeMasterDTORes.Doj;
    //    //employeeMasterDTO.DepartmentId = employeeMasterDTORes.DepartmentId;
    //    //employeeMasterDTO.JobTitleId = employeeMasterDTORes.JobTitleId;
    //    //employeeMasterDTO.ManagerId = employeeMasterDTORes.ManagerId;
    //    //employeeMasterDTO.HODId = employeeMasterDTORes.HODId;
    //    //employeeMasterDTO.OfficialEmail = employeeMasterDTORes.OfficialEmail;
    //    //employeeMasterDTO.IdentityId = employeeMasterDTORes.IdentityId;
    //    //employeeMasterDTO.EmployeeStatus = employeeMasterDTORes.EmployeeStatus;
    //    //employeeMasterDTO.RoleId = employeeMasterDTORes.RoleId;
    //    //employeeMasterDTO.BandId = employeeMasterDTORes.BandId;
    //    //employeeMasterDTO.JoinType = employeeMasterDTORes.JoinType;
    //    //employeeMasterDTO.BasicSalary = employeeMasterDTORes.BasicSalary;
    //    //employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.AnnualBasicSalary;
    //    //employeeMasterDTO.AnnualCtc = employeeMasterDTORes.AnnualCtc;
    //    //employeeMasterDTO.MonthlyCtc = employeeMasterDTORes.MonthlyCtc;
    //    //employeeMasterDTO.OtherCompensation = employeeMasterDTORes.OtherCompensation;
    //    //employeeMasterDTO.SalaryInHand = employeeMasterDTORes.SalaryInHand;
    //    //employeeMasterDTO.CreatedOn = employeeMasterDTORes.CreatedOn;
    //    //employeeMasterDTO.CreatedBy = employeeMasterDTORes.CreatedBy;
    //    //employeeMasterDTO.ModifiedOn = employeeMasterDTORes.ModifiedOn;
    //    //employeeMasterDTO.ModifiedBy = employeeMasterDTORes.ModifiedBy;
    //    //employeeMasterDTO.InfoFillingStatus = employeeMasterDTORes.InfoFillingStatus;
    //    //employeeMasterDTO.IsActive = employeeMasterDTORes.IsActive;
    //    //employeeMasterDTO.Layout = employeeMasterDTORes.Layout;
    //    ////employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
    //    ////employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
    //    ////employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
    //    ////employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
    //    ////employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;
    //    ////employeeMasterDTO.AnnualBasicSalary = employeeMasterDTORes.EmployeeId;


    //    ////employeeMasterDTO.EmployeeCode = employeeMasterDTORes.EmployeeCode;
    //    ////employeeMasterDTO.UnitId = employeeMasterDTORes.UnitId;
    //    ////employeeMasterDTO.ClientId = employeeMasterDTORes.ClientId;
    //    ////employeeMasterDTO.EmailId = employeeMasterDTORes.EmailId;
    //    ////employeeMasterDTO.Base64ProfileImage = employeeMasterDTORes.Base64ProfileImage;
    //    ////employeeMasterDTO.ProfileImage = employeeMasterDTORes.ProfileImage;
    //    ////employeeMasterDTO.RoleId = employeeMasterDTORes.RoleId;


    //    return UserDTO;
    //}

    public static string getContentTypeByExtesnion(string extension)
    {
        string contentType = "";
        switch (extension)
        {
            case ".pdf":
                contentType = "application/pdf";
                break;
            case ".xslx":
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                break;
            case ".docx":
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                break;
            case ".doc":
                contentType = "application/msword";
                break;
            case ".jpg":
                contentType = "image/jpeg";
                break;
            case ".jpeg":
                contentType = "image/jpeg";
                break;
            // and so on
            default:
                throw new ArgumentOutOfRangeException($"Unable to find Content Type for file");
        }
        return contentType;
    }

    public static IEnumerable<DateTime> EachDateBetweenDates(DateTime startDate, DateTime endDate)
    {
        for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
        return date;
    }
}
public enum ColorThemes : int
{
    Default = 1,
    Blue = 2,
    Green = 3,
    Orange = 4,
    Pink = 5,
    Red = 6
}
public enum Gender : int
{
    Male = 1,
    Female = 2
}
