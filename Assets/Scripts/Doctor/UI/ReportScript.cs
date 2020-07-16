using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using UnityEngine.UI;
using System.Diagnostics;
using System;

public class ReportScript : MonoBehaviour
{

    public float x = 0, y = 0;//截屏起始点
    public int width = 1920, height = 1080;//截屏区域

    string picName = ".png";//图片名称
    public string pdfName = ".pdf";//pdf名称    


    

    public static string WallEvaluationReportPath = "Data/报告/评估报告/动作姿势评估";  //保存路径
    public static string EvaluationReportPath = "Data/报告/评估报告/Bobath评估";  //保存路径
    public static string TrainingReportPath = "Data/报告/训练报告/足球守门训练";  //保存路径
    public string ReportPath = "";  // 报告完整路径

    public Rectangle pageSize = PageSize.A4;
    public int pagemode = 1;

    public Toggle WallEvaluationToggle;
    public Toggle EvaluationToggle;
    public Toggle TrainingToggle;

    void OnEnable()
    {
        WallEvaluationToggle = transform.Find("ReportToggle/EvaluationToggle/Evaluation/Evaluations/WallEvaluationToggle").GetComponent<Toggle>();
        EvaluationToggle = transform.Find("ReportToggle/EvaluationToggle/Evaluation/Evaluations/EvaluationToggle").GetComponent<Toggle>();
        TrainingToggle = transform.Find("ReportToggle/TrainingToggle/Training/Trainings/Training").GetComponent<Toggle>();

        x = 382.5f; y = 119.43f;   // 设置起始点
        width = 595; height = 842;  // 设置大小

        StartCoroutine(DelayTime(3));        

    }

    IEnumerator DelayTime(int time)
    {
        yield return new WaitForSeconds(time);

        if (WallEvaluationToggle.isOn)
        {
            SaveWallEvaluationReport();
        }
        else if (EvaluationToggle.isOn)
        {
            SaveEvaluationReport();
        }
        else if (TrainingToggle.isOn)
        {
            SaveTrainingReport();
        }
    }

    public void SaveWallEvaluationReport()
    {
        ReportPath = WallEvaluationReportPath + "/" + DoctorDataManager.instance.doctor.patient.PatientID.ToString() + DoctorDataManager.instance.doctor.patient.PatientName;

        string StartTime = DoctorDataManager.instance.doctor.patient.WallEvaluations[DoctorDataManager.instance.doctor.patient.WallEvaluationIndex].startTime;

        ReportPath += "/" + "第" + (DoctorDataManager.instance.doctor.patient.WallEvaluationIndex + 1).ToString() + "次"
            + "20" + StartTime.Substring(0, 2) + StartTime.Substring(3, 2) + StartTime.Substring(6, 2);

        if (!Directory.Exists(ReportPath))
        {
            Directory.CreateDirectory(ReportPath);
        }

        ReportPath += "/" + "第" + (DoctorDataManager.instance.doctor.patient.WallEvaluationIndex + 1).ToString() + "次"
            + "20" + StartTime.Substring(0, 2) + StartTime.Substring(3, 2) + StartTime.Substring(6, 2);

        if (File.Exists(ReportPath + pdfName) == false)
        {
            StartCoroutine(GetScreenShot(ReportPath));
        }
    }

    public void SaveEvaluationReport()
    {
        ReportPath = EvaluationReportPath + "/" + DoctorDataManager.instance.doctor.patient.PatientID.ToString() + DoctorDataManager.instance.doctor.patient.PatientName;

        ReportPath += "/" + "第" + (DoctorDataManager.instance.doctor.patient.EvaluationIndex + 1).ToString() + "次"
            + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.EvaluationIndex].EvaluationStartTime.Substring(0, 8);

        if (!Directory.Exists(ReportPath))
        {
            Directory.CreateDirectory(ReportPath);
        }

        ReportPath += "/" + "第" + (DoctorDataManager.instance.doctor.patient.EvaluationIndex + 1).ToString() + "次"
            + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.EvaluationIndex].EvaluationStartTime.Substring(0, 8);

        if (File.Exists(ReportPath + pdfName) == false)
        {
            StartCoroutine(GetScreenShot(ReportPath));
        }
    }

    public void SaveTrainingReport()
    {
        ReportPath = TrainingReportPath + "/" + DoctorDataManager.instance.doctor.patient.PatientID.ToString() + DoctorDataManager.instance.doctor.patient.PatientName;

        ReportPath += "/" + "第" + (DoctorDataManager.instance.doctor.patient.TrainingPlayIndex + 1).ToString() + "次"
            + DoctorDataManager.instance.doctor.patient.TrainingPlays[DoctorDataManager.instance.doctor.patient.TrainingPlayIndex].TrainingStartTime.Substring(0, 8);

        if (!Directory.Exists(ReportPath))
        {
            Directory.CreateDirectory(ReportPath);
        }

        ReportPath += "/" + "第" + (DoctorDataManager.instance.doctor.patient.TrainingPlayIndex+1).ToString() + "次"
            + DoctorDataManager.instance.doctor.patient.TrainingPlays[DoctorDataManager.instance.doctor.patient.TrainingPlayIndex].TrainingStartTime.Substring(0, 8);

        if (File.Exists(ReportPath + pdfName) == false)
        {
            StartCoroutine(GetScreenShot(ReportPath));
        }
    }

    IEnumerator GetScreenShot(string ReportPath)
    {
        yield return new WaitForEndOfFrame(); // 停两秒后截图

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, true);
        tex.ReadPixels(new Rect(x, y, width, height), 0, 0, false);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();

        File.WriteAllBytes(ReportPath + picName, bytes);//保存纹理贴图为图片

        yield return new WaitForSeconds(0.1f);

        // 图片转化为pdf
        Document doc = new Document(pageSize, 0, 0, 0, 0);//创建一个A4文档

        PdfWriter.GetInstance(doc, new FileStream(ReportPath + pdfName, FileMode.Create));//该文档创建一个pdf文件实例

        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(new FileStream(ReportPath + picName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));//创建一个Image实例
                                                                                                                                                                         //限制图片不超出A4范围
        if ((image.Height > pageSize.Height) || (image.Width > pageSize.Width))
        {
            image.ScaleToFit(pageSize.Width, pageSize.Height);
        }
        image.Alignment = Element.ALIGN_MIDDLE;
        image.Border = 0;
        // image.SetAbsolutePosition

        doc.Open();
        doc.Add(image);
        doc.Close();
    }

    public void PrintButtonOnClick()
    {
        Invoke("PringPdf", 1f);
    }

    public void PringPdf()
    {
        ReportPath = ReportPath + pdfName;
        ReportPath = ReportPath.Replace("/", "\\");
        //UnityEngine.Debug.Log(ttpdfpath);

        //  Print.PrintTextureByPath("d:\\Test.pdf", 1, string.Empty);//打印一张存在指定路径的图片


        //System.Diagnostics.Process process = new System.Diagnostics.Process(); //系统进程
        //process.StartInfo.CreateNoWindow = false; //不显示调用程序窗口
        //process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;//
        //process.StartInfo.UseShellExecute = true; //采用操作系统自动识别模式
        //process.StartInfo.FileName = ttpdfpath; //要打印的文件路径

        //process.StartInfo.Verb = "print"; //指定对图片执行的动作，打印：print   打开：open …………
        //process.Start(); //开始打印


        Process myProcess = new Process();
        try
        {

            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            myProcess.StartInfo.FileName = ReportPath; //要打印的文件路径
            myProcess.StartInfo.Verb = "print"; //指定对图片执行的动作，打印：print   打开：open …………
            myProcess.Start(); //开始打印

            //UnityEngine.Debug.LogError("screen");

            myProcess.EnableRaisingEvents = true;
            //myProcess.Start();
            //int ExitCode = myProcess.ExitCode;
            //print(ExitCode);
            //print("This prints from selected camera");
        }

        catch (Exception arg)
        {
            //UnityEngine.Debug.LogError(arg);
        }
        finally
        {
            myProcess.Close();
            //UnityEngine.Debug.Log("Texture printing.");
        }

    }

}
