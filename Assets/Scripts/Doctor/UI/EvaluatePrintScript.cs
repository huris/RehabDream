﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
//using LCPrinter;
using UnityEngine.UI;

namespace XCharts
{
    [DisallowMultipleComponent]
    public class EvaluatePrintScript : MonoBehaviour
    {
        // Information
        public Text InformationPatientID;
        public Text InformationPatientName;
        public Text InformationPatientSex;
        public Text InformationPatientAge;
        public Text InformationPatientHeight;
        public Text InformationPatientWeight;
        public Text InformationPatientSymptom;
        public Text InformationPatientDoctor;

        // Evaluation
        public Text EvaluationSuccessCount;
        public Text EvaluationScore;
        public Text EvaluationResult;
        public Text EvaluationRank;
        public Text EvaluationTime;

        // Directions
        public RadarChart DirectionRadarChart; // 雷达图
        public Serie serie, serie1;
        public Text RadarArea; // 雷达图面积
        public Text UponDirection;
        public Text UponRightDirection;
        public Text RightDirection;
        public Text DownRightDirection;
        public Text DownDirection;
        public Text DownLeftDirection;
        public Text LeftDirection;
        public Text UponLeftDirection;


        void OnEnable()
        {
            if(DoctorDataManager.instance.doctor.patient.Evaluations != null && DoctorDataManager.instance.doctor.patient.Evaluations.Count > 0)
            {
                // Information
                InformationPatientID = transform.Find("Background/Information/PatientInfo/ID/PatientID").GetComponent<Text>();
                InformationPatientName = transform.Find("Background/Information/PatientInfo/Name/PatientName").GetComponent<Text>();
                InformationPatientSex = transform.Find("Background/Information/PatientInfo/Sex/PatientSex").GetComponent<Text>();
                InformationPatientAge = transform.Find("Background/Information/PatientInfo/Age/PatientAge").GetComponent<Text>();
                InformationPatientHeight = transform.Find("Background/Information/PatientInfo/Height/PatientHeight").GetComponent<Text>();
                InformationPatientWeight = transform.Find("Background/Information/PatientInfo/Weight/PatientWeight").GetComponent<Text>();
                InformationPatientSymptom = transform.Find("Background/Information/PatientInfo/Symptom/PatientSymptom").GetComponent<Text>();
                InformationPatientDoctor = transform.Find("Background/Information/PatientInfo/Doctor/PatientDoctor").GetComponent<Text>();

                // Evaluation
                EvaluationRank = transform.Find("Background/Evaluation/EvaluationInfo/Rank/EvaluationRank").GetComponent<Text>();
                EvaluationSuccessCount = transform.Find("Background/Evaluation/EvaluationInfo/SuccessCount/EvaluationSuccessCount").GetComponent<Text>();
                EvaluationScore = transform.Find("Background/Evaluation/EvaluationInfo/Score/EvaluationScore").GetComponent<Text>();
                EvaluationTime = transform.Find("Background/Evaluation/EvaluationInfo/Time/EvaluationTime").GetComponent<Text>();
                EvaluationResult = transform.Find("Background/Evaluation/EvaluationInfo/Result/EvaluationResult").GetComponent<Text>();

                InformationPatientID.text = DoctorDataManager.instance.doctor.patient.PatientID.ToString();
                InformationPatientName.text = DoctorDataManager.instance.doctor.patient.PatientName;
                InformationPatientSex.text = DoctorDataManager.instance.doctor.patient.PatientSex;
                InformationPatientAge.text = DoctorDataManager.instance.doctor.patient.PatientAge.ToString() + "岁";

                if (DoctorDataManager.instance.doctor.patient.PatientHeight == -1) InformationPatientHeight.text = "未填写";
                else InformationPatientHeight.text = DoctorDataManager.instance.doctor.patient.PatientHeight.ToString() + "CM";

                if (DoctorDataManager.instance.doctor.patient.PatientWeight == -1) InformationPatientWeight.text = "未填写";
                else InformationPatientWeight.text = DoctorDataManager.instance.doctor.patient.PatientWeight.ToString() + "KG                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          ";

                InformationPatientSymptom.text = DoctorDataManager.instance.doctor.patient.PatientSymptom;
                InformationPatientDoctor.text = DoctorDatabaseManager.instance.ReadDoctorIDInfo(DoctorDataManager.instance.doctor.patient.PatientDoctorID).DoctorName;

                //if (DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].EvaluationScore == 0.0f)
                //{
                //    DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].SetEvaluationScore();
                //}

                EvaluationScore.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].EvaluationScore.ToString("0.00");
                EvaluationRank.text = "(           )";
                EvaluationSuccessCount.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].SuccessCount.ToString() + "/" +
                    DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].GameCount.ToString() + "(" +
                    (1.0f * DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].SuccessCount /
                     DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].GameCount * 100).ToString("0.00") + "%)";
                EvaluationTime.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].TrainingStartTime;
                //EvaluationResult.text

                DirectionRadarChart = transform.Find("Background/Chart/Directions/RadarChart").GetComponent<RadarChart>();
                if (DirectionRadarChart == null) DirectionRadarChart = transform.Find("Background/Chart/Directions/RadarChart").gameObject.AddComponent<RadarChart>();
                DirectionRadarChart.ClearData();
                DirectionRadarChart.AddData(0, DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.GetDirections());

                RadarArea = transform.Find("Background/Chart/Directions/RadarArea/Text").GetComponent<Text>();
                UponDirection = transform.Find("Background/Chart/Directions/DirectionFrame/UponDirection/Upon/Text").GetComponent<Text>();
                UponRightDirection = transform.Find("Background/Chart/Directions/DirectionFrame/UponRightDirection/UponRight/Text").GetComponent<Text>();
                RightDirection = transform.Find("Background/Chart/Directions/DirectionFrame/RightDirection/Right/Text").GetComponent<Text>();
                DownRightDirection = transform.Find("Background/Chart/Directions/DirectionFrame/DownRightDirection/DownRight/Text").GetComponent<Text>();
                DownDirection = transform.Find("Background/Chart/Directions/DirectionFrame/DownDirection/Down/Text").GetComponent<Text>();
                DownLeftDirection = transform.Find("Background/Chart/Directions/DirectionFrame/DownLeftDirection/DownLeft/Text").GetComponent<Text>();
                LeftDirection = transform.Find("Background/Chart/Directions/DirectionFrame/LeftDirection/Left/Text").GetComponent<Text>();
                UponLeftDirection = transform.Find("Background/Chart/Directions/DirectionFrame/UponLeftDirection/UponLeft/Text").GetComponent<Text>();

                //RadarArea.text = DoctorDataManager.instance.doctor.patient.Evaluations[DoctorDataManager.instance.doctor.patient.Evaluations.Count - 1].direction.GetRadarArea().ToString("0.00");
                //UponDirection.text = DoctorDataManager.instance.doctor.patient.Evaluations

            }

        }

        void Update()
        {

        }
    }
}
//public class EvaluatePrintScript : MonoBehaviour
//{
//    string mode = "RunTime";//模式:RunTime Release
//    int x = 0, y = 0;//截屏起始点
//    int width = 1920, height = 1080;//截屏区域

//    string picName = "ScreenShot.png";//图片名称
//    //  string picPath = "Assets/StreamingAssets";//图片路径
//    // string pdfPath = "Assets/StreamingAssets";//pdf路径

//    string picPath;
//    string pdfPath;
//    string pdfName = "Test.pdf";//pdf名称    
//    int pdfMode = 0;
//    //0是生成图片后生成pdf 打印   1是直接生成pdf打印  2是直接打印二进制数据


//    //Rectangle pageSize = new Rectangle(1920, 1080);
//    Rectangle pageSize = PageSize.A4.Rotate();
//    private int pagemode = 1;
//    //屏幕模式

//    void Awake()
//    {
//        width = Screen.width;
//        height = Screen.height;
//        if (pagemode == 0)
//        {
//            pageSize = new Rectangle(width, height);
//        }
//        else
//        {
//            pageSize = PageSize.A4.Rotate();
//        }

//        picPath = Application.streamingAssetsPath;
//        pdfPath = Application.streamingAssetsPath;
//        // mPdfManager = new PDFMakerManager();
//        // mPdfManager.setpageSize(pageSize);
//        Init();
//    }
//    private void Start()
//    {

//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            saveImg();
//        }
//    }


//    /// <summary>
//    /// 初始化
//    /// </summary>
//    void Init()
//    {
//        string xmlPath = Application.streamingAssetsPath + "/Config.xml";
//        mode = Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "Mode" });

//        if (mode == "Release")
//        {
//            x = int.Parse(Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "X" }));
//            y = int.Parse(Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "Y" }));
//            width = int.Parse(Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "Width" }));
//            height = int.Parse(Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "Height" }));
//            picPath = Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "PicturePath" });
//            picName = Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "PictureName" });
//            pdfPath = Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "PdfPath" });
//            pdfName = Xml.ReadElement(xmlPath, new string[] { "configuration", "ScreenShot2Pdf", "parameters", "PdfName" });
//        }
//    }
//    public void saveImg()
//    {
//        StartCoroutine(GetScreenShot());
//    }
//    /// <summary>
//    /// 得到屏幕截图
//    /// </summary>
//    /// <returns></returns>
//    IEnumerator GetScreenShot()
//    {
//        yield return new WaitForEndOfFrame();

//        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, true);
//        tex.ReadPixels(new Rect(x, y, width, height), 0, 0, false);
//        tex.Apply();

//        byte[] bytes = tex.EncodeToPNG();
//        if (pdfMode == 0)
//        {
//            if (!Directory.Exists(picPath))
//                Directory.CreateDirectory(picPath);
//            File.WriteAllBytes(picPath + "/" + picName, bytes);//保存纹理贴图为图片
//        }


//        yield return new WaitForSeconds(0.1f);
//        if (pdfMode == 0)
//        {
//            ConvertPicture2PDF();
//        }
//        else if (pdfMode == 1)
//        {
//            PrintImgBytesPdf(bytes);
//        }
//        else if (pdfMode == 2)
//        {
//            PrintImgBytes(bytes);
//        }
//    }


//    void ConvertPicture2PDF()
//    {

//        //  UnityEngine.Debug.Log(pageSize);

//        // Document doc = new Document(PageSize.A4.Rotate());//创建一个A4文档       
//        Document doc = new Document(pageSize, 0, 0, 0, 0);//创建一个A4文档
//        PdfWriter.GetInstance(doc, new FileStream(pdfPath + "/" + pdfName, FileMode.Create));//该文档创建一个pdf文件实例
//        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(new FileStream(picPath + "/" + picName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));//创建一个Image实例
//                                                                                                                                                                      //限制图片不超出A4范围
//                                                                                                                                                                      //    image.ScaleToFit(PageSize.A4.Width - 25, PageSize.A4.Height - 25);
//                                                                                                                                                                      //}
//        if ((image.Height > pageSize.Height) || (image.Width > pageSize.Width))
//        {
//            image.ScaleToFit(pageSize.Width, pageSize.Height);
//        }
//        image.Alignment = Element.ALIGN_MIDDLE;
//        image.Border = 0;
//        // image.SetAbsolutePosition

//        doc.Open();
//        doc.Add(image);
//        doc.Close();


//        // System.Diagnostics.Process.Start("mspaint.exe", "\"/pt \"" + ttpdfpath);
//        //System.Diagnostics.Process.Start("mspaint.exe", "\"/pt " + ttpdfpath+ "\"");

//        // N:/ unitytproject temp / printPDF / Assets / StreamingAssets / Test.pdf


//        Invoke("PringPdf", 1f);
//    }



//    public void PrintImgBytesPdf(byte[] _b)
//    {
//        string filePath = pdfPath + "/" + pdfName;
//        Document document = new Document(pageSize, 0, 0, 0, 0);
//        PdfWriter.GetInstance(
//         document,
//          new FileStream(filePath, FileMode.Create)
//        );
//        document.Open();
//        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(_b);
//        if ((image.Height > pageSize.Height) || (image.Width > pageSize.Width))
//        {
//            image.ScaleToFit(pageSize.Width, pageSize.Height);
//        }
//        //image.ScaleAbsolute(image.Width, image.Height);
//        // image.Alignment = Image.LEFT_BORDER;
//        image.Alignment = Element.ALIGN_MIDDLE;//居中对齐

//        document.Add(image);
//        document.Close();
//        //Debug.Log("  打印完成！文件保存在：" + filePath);

//        Invoke("PringPdf", 1f);
//    }
//    public void PringPdf()
//    {
//        string ttpdfpath = pdfPath + "/" + pdfName;
//        ttpdfpath = ttpdfpath.Replace("/", "\\");
//        UnityEngine.Debug.Log(ttpdfpath);

//        //  Print.PrintTextureByPath("d:\\Test.pdf", 1, string.Empty);//打印一张存在指定路径的图片


//        //System.Diagnostics.Process process = new System.Diagnostics.Process(); //系统进程
//        //process.StartInfo.CreateNoWindow = false; //不显示调用程序窗口
//        //process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;//
//        //process.StartInfo.UseShellExecute = true; //采用操作系统自动识别模式
//        //process.StartInfo.FileName = ttpdfpath; //要打印的文件路径

//        //process.StartInfo.Verb = "print"; //指定对图片执行的动作，打印：print   打开：open …………
//        //process.Start(); //开始打印


//        Process myProcess = new Process();
//        try
//        {

//            myProcess.StartInfo.CreateNoWindow = true;
//            myProcess.StartInfo.UseShellExecute = true;
//            myProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

//            myProcess.StartInfo.FileName = ttpdfpath; //要打印的文件路径
//            myProcess.StartInfo.Verb = "print"; //指定对图片执行的动作，打印：print   打开：open …………
//            myProcess.Start(); //开始打印


//            myProcess.EnableRaisingEvents = true;
//            myProcess.Start();
//            int ExitCode = myProcess.ExitCode;
//            print(ExitCode);
//            print("This prints from selected camera");
//        }

//        catch (Exception arg)
//        {
//            //UnityEngine.Debug.LogError(arg);
//        }
//        finally
//        {
//            myProcess.Close();
//            //UnityEngine.Debug.Log("Texture printing.");
//        }

//    }

//    private void PrintImgBytes(byte[] bytes)
//    {


//        string filename = picPath + "/" + picName;
//        string printPath = filename.Replace("/", "\\");
//        UnityEngine.Debug.Log(printPath);

//        File.WriteAllBytes(filename, bytes);
//        print("This prints from selected camera");

//        try
//        {
//            Process myProcess = new Process();
//            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
//            myProcess.StartInfo.CreateNoWindow = true;
//            myProcess.StartInfo.UseShellExecute = false;
//            myProcess.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
//            string path = "mspaint.exe /p " + "\"" + printPath + "\"";        //here you define your custom path
//            print(path);
//            myProcess.StartInfo.Arguments = "/c" + path;
//            myProcess.EnableRaisingEvents = true;
//            myProcess.Start();
//            int ExitCode = myProcess.ExitCode;
//            print(ExitCode);
//            print("This prints from selected camera");
//        }
//        catch (Exception e)
//        {

//        }
//    }

//    public void SetpdfMode(int i)
//    {
//        pdfMode = i;
//    }
//}