using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using UnityEngine.UI;

public class ReportScript : MonoBehaviour
{

    public int x = 0, y = 0;//截屏起始点
    public int width = 1920, height = 1080;//截屏区域

    string picName = ".png";//图片名称
    public string pdfName = ".pdf";//pdf名称    

    public static string EvaluationReportPath = "Data/报告/评估报告";  //保存路径
    public static string TrainingReportPath = "Data/报告/训练报告";  //保存路径

    public Rectangle pageSize = PageSize.A4;
    public int pagemode = 1;

    public Toggle EvaluationToggle;
    public Toggle TrainingToggle;

    void OnEnable()
    {
        EvaluationToggle = transform.Find("Evaluation").GetComponent<Toggle>();
        TrainingToggle = transform.Find("Training").GetComponent<Toggle>();

        if(EvaluationToggle.isOn)
        {
            SaveEvaluationReport();
        }
        else if (TrainingToggle.isOn)
        {
            SaveTrainingReport();
        }

    }

    public void SaveEvaluationReport()
    {
        x = 0; y = 0;   // 设置起始点
        width = 595; height = 842;  // 设置大小

        string ReportPath = EvaluationReportPath + "/" + DoctorDataManager.instance.doctor.patient.PatientID.ToString() + DoctorDataManager.instance.doctor.patient.PatientName;

        if (!Directory.Exists(ReportPath))
        {
            Directory.CreateDirectory(ReportPath);
        }

        ReportPath += "/" + "第" + DoctorDataManager.instance.doctor.patient.Evaluations.Count + "次"
            + DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].TrainingStartTime;

        StartCoroutine(GetScreenShot(ReportPath));
    }

    public void SaveTrainingReport()
    {
        x = 1919 - width; y = 1079 - height;   // 设置起始点
        width = 595; height = 842;  // 设置大小

        string ReportPath = TrainingReportPath + "/" + DoctorDataManager.instance.doctor.patient.PatientID.ToString() + DoctorDataManager.instance.doctor.patient.PatientName;

        if (!Directory.Exists(ReportPath))
        {
            Directory.CreateDirectory(ReportPath);
        }

        ReportPath += "/" + "第" + DoctorDataManager.instance.doctor.patient.trainingPlays.Count + "次"
            + DoctorDataManager.instance.doctor.patient.trainingPlays[DoctorDataManager.instance.doctor.patient.trainingPlays.Count - 1].TrainingStartTime;

        StartCoroutine(GetScreenShot(ReportPath));
    }

    IEnumerator GetScreenShot(string ReportPath)
    {
        yield return new WaitForEndOfFrame();

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, true);
        tex.ReadPixels(new Rect(x, y, width, height), 0, 0, false);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();

        //File.WriteAllBytes(ReportPath + picName, bytes);//保存纹理贴图为图片

        yield return new WaitForSeconds(0.1f);

        // 图片转化为pdf
        Document doc = new Document(pageSize, 0, 0, 0, 0);//创建一个A4文档
        PdfWriter.GetInstance(doc, new FileStream(ReportPath + "/" + pdfName, FileMode.Create));//该文档创建一个pdf文件实例
        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(new FileStream(ReportPath + "/" + picName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));//创建一个Image实例
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
}
