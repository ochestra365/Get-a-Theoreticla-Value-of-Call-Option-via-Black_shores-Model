using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WpfSMSApp.View;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

namespace WpfSMSApp.View.Store
{
    /// <summary>
    /// MyAccountView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StoreList : Page
    {
        public StoreList()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Store 테이블 데이터 읽어와야 함.
                List<Model.Store> stores = new List<Model.Store>();
                List<Model.StockStores> stockstores = new List<Model.StockStores>();
                stores = Logic.DataAccess.GetStores();//수영 1

                //stores 데이터를 stockStores로 복사
                foreach (Model.Store item in stores)
                {
                    var store = new Model.StockStores()
                    {
                        StoreID = item.StoreID,
                        StoreName = item.StoreName,
                        StoreLocation = item.StoreLocation,
                        ItemStatus = item.ItemStatus,
                        TagID = item.TagID,
                        BarcodeID = item.BarcodeID,
                        StocksQuantity = 0
                    };
                    /*store.StockQuantity = Logic.DataAccess.GetStocks().Where(t => t.StoreID.Equals(store.StoreID)).Count();

                    stockStores.Add(store);*/
                }
                this.DataContext = stockstores;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 StoreList Loaded: {ex}");
                throw ex;
            }
        }

        private void BtnEditMyAccount_Click(object sender, RoutedEventArgs e)//여기서 잘못되었다. 네비게이션 기능이 겁나 좋은 기능이다.
        {
            try
            {
                //NavigationService.Navigate(new UserList());//이거 아닌데? MyAccount에서 시스템 자원 새로 할당하는 건데? 뭐지? // 답은 내가 View의 Account에서 생성하지 않고 프로젝트 영역에 cs를 생성해서 그렇다. 페이지 뺸 거 부터 고쳐서 나가야함.
                //사소한 실수가 네임스페이스를 다 꼬이게 만들 수 있어..
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnAccount_Click : {ex}");//NLOG를 활용하는 파트이다.
            }
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PDF File(*.pdf)|*.pdf";
            saveDialog.FileName = "";
            saveDialog.ShowDialog();
            //위에는 저장하는 파트
            if (saveDialog.ShowDialog() == true)
            {
                //pdf변환
                try
                {
                    //0. PDF 사용 폰트 설정.
                    string nanumPath = Path.Combine(Environment.CurrentDirectory, @"NanumGothic.ttf");
                    BaseFont nanumBase = BaseFont.CreateFont(nanumPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    var nanumTitle = new iTextSharp.text.Font(nanumBase, 20f);  //20 타이틀용 나눔폰트폰트라는 폴더가 2개가 있으니 명확하게 사용자가 어느 폰트를 사용할 건지 지정해줘야 한다. 
                    var nanumContent = new iTextSharp.text.Font(nanumBase, 12f);// 12내용 나눔폰트,

                    //iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                    string pdfFilePath = saveDialog.FileName;

                    // 1. pdf 객체생성시작
                    Document pdfDoc = new Document(PageSize.A4);

                    // 2. pdf 내용 만들기
                    /* string nanumttf = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), @"Fonts\NanumGothic.ttf");
                     BaseFont nanumBase = BaseFont.CreateFont(nanumttf, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                     var nanumFont = new iTextSharp.text.Font(nanumBase, 16f);//한글로 안나오는데에 대해서 하는 거임*/
                    Paragraph title = new Paragraph($"부경대 재고 관리 시스템(SMS)\n", nanumTitle);//using문 중 모호한 참조가 있다면 지워줘야 한다.
                    Paragraph subtitle = new Paragraph($"사용자리스트 exported : {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n\n", nanumContent);//using문 중 모호한 참조가 있다면 지워줘야 한다.
                    PdfPTable pdfTable = new PdfPTable(GrdData.Columns.Count);
                    pdfTable.WidthPercentage = 100;//전체 사이즈 다 쓰는 것이다.
                    // 그리드 헤더 작업
                    var index = 0;
                    foreach (DataGridColumn column in GrdData.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString(), nanumContent));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfTable.AddCell(cell);
                    }
                    float[] columnWdith = new float[] { 7f, 15f, 10f, 15f, 28f, 10f, 10f };
                    pdfTable.SetWidths(columnWdith);
                    //그리드 Row 작업
                    foreach (var item in GrdData.Items)
                    {
                        if(item is Model.User)
                        {
                            var temp = item as Model.User;
                            //UserId
                            PdfPCell cell = new PdfPCell(new Phrase(temp.UserID.ToString(), nanumContent));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            pdfTable.AddCell(cell);
                            //UserIdentityNumber
                            cell = new PdfPCell(new Phrase(temp.UserIdentityNumber.ToString(), nanumContent));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfTable.AddCell(cell);
                            //UserSurName
                            cell = new PdfPCell(new Phrase(temp.UserSurname.ToString(), nanumContent));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfTable.AddCell(cell);
                            //UserName
                            cell = new PdfPCell(new Phrase(temp.UserName.ToString(), nanumContent));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfTable.AddCell(cell);
                            //UserEmail
                            cell = new PdfPCell(new Phrase(temp.UserEmail.ToString(), nanumContent));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfTable.AddCell(cell);
                            //UserAdmin
                            cell = new PdfPCell(new Phrase(temp.UserAdmin.ToString(), nanumContent));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfTable.AddCell(cell);
                            //UserActivated
                            cell = new PdfPCell(new Phrase(temp.UserActivated.ToString(), nanumContent));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pdfTable.AddCell(cell);
                        }
                    }

                    // 3. pdf 파일생성
                    using (FileStream stream = new FileStream(pdfFilePath, FileMode.OpenOrCreate))
                    {
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        // 2번에서 만든 내용을 추가한다.
                        pdfDoc.Add(title);
                        pdfDoc.Add(subtitle);
                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                        stream.Close();//option
                    }
                    Commons.ShowMessageAsync("PDF변환", "PDF 익스포트 성공했습니다.");
                }
                catch (Exception ex)
                {
                    Commons.LOGGER.Error($"예외발생 BtnExportPdf_Click: {ex}");
                }
            } 
        }

        private void BtnExportPdf_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddStore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new AddStore());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외 발생  BtnAddStore_Click : {ex}");
                throw ex;
            }
        }

        private void BtnEditStore_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItem == null)
            {
                Commons.ShowMessageAsync("참고수정", "수정할 창고를 선택하세요");
                return;
            }
            try
            {
                var storeId = (GrdData.SelectedItem as Model.Store).StoreID;
                NavigationService.Navigate(new EidtStore());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외 발생  EidtStore_Click : {ex}");
            }
        }

        private void BtnExportEXCEL_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel File (x.xlsx|*.xlsx";//엑셀확장자 필터링
            dialog.FileName = "";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IWorkbook workbook = new XSSFWorkbook();//xlsx용 //new HSSFWorkbook();//xls(이전버전용)
                    ISheet sheet = workbook.CreateSheet("Sheet1");//이름변경 가능
                                                                  //헤더row
                    IRow rowHeader = sheet.CreateRow(0);
                    ICell cell = rowHeader.CreateCell(0);
                    cell.SetCellValue("순번");
                    cell = rowHeader.CreateCell(1);
                    cell.SetCellValue("창고명");
                    cell = rowHeader.CreateCell(2);
                    cell.SetCellValue("창고위치");
                    cell = rowHeader.CreateCell(3);
                    cell.SetCellValue("재고수");

                    //요거는 페이지 로드 부분 고쳐야 나옴 놓친 부분이 있어서 고쳐야함.
                    for (int i = 0; i < GrdData.Items.Count; i++)
                    {
                        IRow row = sheet.CreateRow(i + 1); // 
                        if (GrdData.Items[i] is Model.StockStores)
                        {
                            var stockStore = GrdData.Items[i] as Model.StockStores;
                            ICell dataCell = row.CreateCell(0);
                            dataCell.SetCellValue(stockStore.StoreID);
                            dataCell = row.CreateCell(1);
                            dataCell.SetCellValue(stockStore.StoreName);
                            dataCell = row.CreateCell(2);
                            dataCell.SetCellValue(stockStore.StoreLocation);
                            /*dataCell = row.CreateCell(3);
                            dataCell.SetCellValue(stockStore.StockQuantity);*/
                        }
                        //파일저장
                        using (var fs = new FileStream(dialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))//디버그 잡아서 마우스 올려보면 매개변수에 데이터가 스택이 되었는 지 알 수 있다. 평상시 유저에게는 FileName으로 인지되지만 컴파일러는 경로로 인식함을 알 수 있따다.
                        {
                            workbook.Write(fs);
                        }
                        Commons.ShowMessageAsync("엑셀저장", "엑셀export 성공!");
                    }
                }
                catch (Exception ex)
                {
                    Commons.ShowMessageAsync("예외", $"예외발생 {ex}");
                    Commons.LOGGER.Error($"예외 발생 : {ex}");
                }
            }
        }
    }
}
//StockQuantity 정보를 읽을 수가 없고 Page_Load파트를 수정하니 출력화면에 창고정보가 날라오지 않는다 왜그렇지