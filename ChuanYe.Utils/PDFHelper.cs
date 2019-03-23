using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace ChuanYe.Utils
{
    /// <summary>
    /// PDF文档操作类
    /// </summary>
    //------------------调用--------------------------
    //PDFOperation pdf = new PDFOperation();
    //pdf.Open(new FileStream(path, FileMode.Create));
    //pdf.SetBaseFont(@"C:\Windows\Fonts\SIMHEI.TTF");
    //pdf.AddParagraph("测试文档（生成时间：" + DateTime.Now + "）", 15, 1, 20, 0, 0);
    //pdf.Close();
    //-------------------------------
    public class PDFHelper
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public PDFHelper()
        {
            rect = PageSize.A4;
            document = new Document(rect);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">页面大小(如"A4")</param>
        public PDFHelper(string type)
        {
            SetPageSize(type);
            document = new Document(rect);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">页面大小(如"A4")</param>
        /// <param name="marginLeft">内容距左边框距离</param>
        /// <param name="marginRight">内容距右边框距离</param>
        /// <param name="marginTop">内容距上边框距离</param>
        /// <param name="marginBottom">内容距下边框距离</param>
        public PDFHelper(string type, float marginLeft, float marginRight, float marginTop, float marginBottom)
        {
            SetPageSize(type);
            document = new Document(rect, marginLeft, marginRight, marginTop, marginBottom);
        }
        #endregion
        #region 私有字段
        private Font font;
        private Rectangle rect;  //文档大小
        public Document document;//文档对象
        public BaseFont basefont;//字体
        #endregion
        #region 设置字体
        /// <summary>
        /// 设置字体
        /// </summary>
        public void SetBaseFont(string path)
        {
            basefont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        }
        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="size">字体大小</param>
        public void SetFont(float size)
        {
            font = new Font(basefont, size);
        }
        #endregion
        #region 设置页面大小
        /// <summary>
        /// 设置页面大小
        /// </summary>
        /// <param name="type">页面大小(如"A4")</param>
        public void SetPageSize(string type)
        {
            switch (type.Trim())
            {
                case "A4":
                    rect = PageSize.A4;
                    break;
                case "A8":
                    rect = PageSize.A8;
                    break;
            }
        }
        #endregion
        #region 实例化文档
        /// <summary>
        /// 实例化文档
        /// </summary>
        /// <param name="os">文档相关信息（如路径，打开方式等）</param>
        public void GetInstance(Stream os)
        {
            PdfWriter.GetInstance(document, os);
        }
        #endregion
        #region 打开文档对象
        /// <summary>
        /// 打开文档对象
        /// </summary>
        /// <param name="os">文档相关信息（如路径，打开方式等）</param>
        public void Open(Stream os)
        {
            GetInstance(os);
            document.Open();
        }
        #endregion
        #region 关闭打开的文档
        /// <summary>
        /// 关闭打开的文档
        /// </summary>
        public void Close()
        {
            document.Close();
        }
        #endregion
        #region 添加段落

        /// <summary>
        /// 添加段落
        /// </summary>
        /// <param name="par"></param>
        /// <param name="fontsize"></param>
        public void AddParagraph(Paragraph par,float fontsize)
        {
            SetFont(fontsize);
            document.Add(par);
        }


        public void AddParagraph(Paragraph par, float fontsize,int Alignment
                                     , float SpacingAfter, float SpacingBefore, float MultipliedLeading)
        {
            SetFont(fontsize);
            par.Alignment = Alignment;
            par.SpacingAfter = SpacingAfter;
            par.SpacingBefore = SpacingBefore;
            par.MultipliedLeading = MultipliedLeading;
            document.Add(par);
        }


        /// <summary>
        /// 添加段落
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="fontsize">字体大小</param>
        public void AddParagraph(string content, float fontsize)
        {
            SetFont(fontsize);
            Paragraph pra = new Paragraph(content, font);
            document.Add(pra);
        }
        /// <summary>
        /// 添加段落
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="Alignment">对齐方式（1为居中，0为居左，2为居右）</param>
        /// <param name="SpacingAfter">段后空行数（0为默认值）</param>
        /// <param name="SpacingBefore">段前空行数（0为默认值）</param>
        /// <param name="MultipliedLeading">行间距（0为默认值）</param>
        public void AddParagraph(string content, float fontsize, int Alignment, float SpacingAfter, float SpacingBefore, float MultipliedLeading)
        {
            SetFont(fontsize);
            Paragraph pra = new Paragraph(content, font);
            pra.Alignment = Alignment;
            if (SpacingAfter != 0)
            {
                pra.SpacingAfter = SpacingAfter;
            }
            if (SpacingBefore != 0)
            {
                pra.SpacingBefore = SpacingBefore;
            }
            if (MultipliedLeading != 0)
            {
                pra.MultipliedLeading = MultipliedLeading;
            }
            document.Add(pra);
        }
        #endregion
        #region 添加图片
        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <param name="Alignment">对齐方式（1为居中，0为居左，2为居右）</param>
        /// <param name="newWidth">图片宽（0为默认值，如果宽度大于页宽将按比率缩放）</param>
        /// <param name="newHeight">图片高</param>
        public void AddImage(string path, int Alignment, float newWidth, float newHeight)
        {
            Image img = Image.GetInstance(path);
            img.Alignment = Alignment;
            if (newWidth != 0)
            {
                img.ScaleAbsolute(newWidth, newHeight);
            }
            else
            {
                if (img.Width > PageSize.A4.Width)
                {
                    img.ScaleAbsolute(rect.Width, img.Width * img.Height / rect.Height);
                }
            }
            document.Add(img);
        }
        #endregion
        #region 添加链接、点
        /// <summary>
        /// 添加链接
        /// </summary>
        /// <param name="Content">链接文字</param>
        /// <param name="FontSize">字体大小</param>
        /// <param name="Reference">链接地址</param>
        public void AddAnchorReference(string Content, float FontSize, string Reference)
        {
            SetFont(FontSize);
            Anchor auc = new Anchor(Content, font);
            auc.Reference = Reference;
            document.Add(auc);
        }
        /// <summary>
        /// 添加链接点
        /// </summary>
        /// <param name="Content">链接文字</param>
        /// <param name="FontSize">字体大小</param>
        /// <param name="Name">链接点名</param>
        public void AddAnchorName(string Content, float FontSize, string Name)
        {
            SetFont(FontSize);
            Anchor auc = new Anchor(Content, font);
            auc.Name = Name;
            document.Add(auc);
        }
        #endregion
        #region 添加块(段落中)
        public void AddChunkByParagraph(Paragraph par, string content, Font font)
        {
            Chunk ck = new Chunk(content, font);
            par.Add(ck);
        }
        #endregion
        #region 表格
        public void AddTable(PdfPTable table)
        {
            document.Add(table);
        }

        public void AddCellByTable(PdfPTable table
                                   , string content, Font font
                                   , int alignment, int colspan
                                   , int minimumHeight
                                   , int border = 1)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.MinimumHeight = minimumHeight;
            cell.Border = border;
            cell.Colspan = colspan;
            cell.HorizontalAlignment = alignment;
            table.AddCell(cell);
        }

        public PdfPTable CreateTable(int numColumns, float width = 90.0f)
        {
            PdfPTable table = new PdfPTable(numColumns);
            table.WidthPercentage = width;
            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="font">字体</param>
        /// <param name="colspan">合并列</param>
        /// <param name="minimumHeight">行最小宽度</param>
        /// <param name="paddingLeft">距离左边距离</param>
        /// <param name="horizontalAlignment">横向位置</param>
        /// <param name="verticalAlignment">纵向位置</param>
        /// <param name="border">边框类型</param>
        /// <returns></returns>
        public PdfPCell CreateCell(string content, Font font, int colspan, float minimumHeight
                                  , float paddingLeft = 5.0f, int horizontalAlignment = Element.ALIGN_BOTTOM
                                  , int verticalAlignment = Element.ALIGN_MIDDLE, int border = Rectangle.BOX)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, font));
            cell.MinimumHeight = minimumHeight;
            cell.Border = border;
            cell.Colspan = colspan;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.VerticalAlignment = verticalAlignment;
            cell.PaddingLeft = paddingLeft;
            return cell;
        }
        #endregion
    }
}