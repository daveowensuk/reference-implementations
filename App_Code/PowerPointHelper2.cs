using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using A = DocumentFormat.OpenXml.Drawing;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using Picture = DocumentFormat.OpenXml.Presentation.Picture;
using Vt = DocumentFormat.OpenXml.VariantTypes;

public class PageInfo {
  public Bitmap Bitmap {
    get;
    set;
  }
  public string Title {
    get;
    set;
  }
  public bool IsChart {
    get;
    set;
  }
}

public class PowerPointHelper2 {
  // Creates a PresentationDocument.
  public void CreatePackage(string filePath) {
    using (PresentationDocument package = PresentationDocument.Create(filePath, PresentationDocumentType.Presentation)) {
      CreateParts(package, null);
    }
  }

  public static byte[] ImageToByte(Image img) {
    ImageConverter converter = new ImageConverter();
    return (byte[])converter.ConvertTo(img, typeof(byte[]));
  }

  // Generates content of imagePart1.
  private static void GenerateImagePartContent(ImagePart imagePart, Bitmap bitmap) {
    imagePart.FeedData(new MemoryStream(ImageToByte(bitmap)));
  }

  /*public static IEnumerable<ValidationErrorInfo> IsDocumentValid(PresentationDocument mydoc)
  {
  	OpenXmlValidator validator = new OpenXmlValidator();
  	var errors = validator.Validate(mydoc);
  	var errorsList = new List<ValidationErrorInfo>(errors);
  	return errorsList.Count == 0 ? null : errorsList;
  }*/

  // Adds child parts and generates content of the specified part.
  public void CreateParts(PresentationDocument document, List<PageInfo> bitmaps) {
    ThumbnailPart thumbnailPart1 = document.AddNewPart<ThumbnailPart>("image/jpeg", "rId2");
    GenerateThumbnailPart1Content(thumbnailPart1);

    var idList = new List<string>();
    for (var i = 0; i<bitmaps.Count; i++) {
      if (i==0) {
        idList.Add("rId2");
      } else {
        idList.Add("rrId"+(2+i));
      }
    }

    PresentationPart presentationPart1 = document.AddPresentationPart();
    GeneratePresentationPart1Content(presentationPart1, idList);

    TableStylesPart tableStylesPart1 = presentationPart1.AddNewPart<TableStylesPart>("rId8");
    GenerateTableStylesPart1Content(tableStylesPart1);

    SlidePart slidePart1 = presentationPart1.AddNewPart<SlidePart>("rId2");
    GenerateSlidePartContent(slidePart1, 0, bitmaps[0]);
    ImagePart imagePart1 = slidePart1.AddNewPart<ImagePart>("image/jpeg", "rId2");
    GenerateImagePartContent(imagePart1, bitmaps[0].Bitmap);

    #region layouts

    SlideLayoutPart slideLayoutPart1 = slidePart1.AddNewPart<SlideLayoutPart>("rId1");
    GenerateSlideLayoutPart1Content(slideLayoutPart1);

    SlideMasterPart slideMasterPart1 = slideLayoutPart1.AddNewPart<SlideMasterPart>("rId1");
    GenerateSlideMasterPart1Content(slideMasterPart1);

    SlideLayoutPart slideLayoutPart2 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId8");
    GenerateSlideLayoutPart2Content(slideLayoutPart2);

    slideLayoutPart2.AddPart(slideMasterPart1, "rId1");

    SlideLayoutPart slideLayoutPart3 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId3");
    GenerateSlideLayoutPart3Content(slideLayoutPart3);

    slideLayoutPart3.AddPart(slideMasterPart1, "rId1");

    SlideLayoutPart slideLayoutPart4 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId7");
    GenerateSlideLayoutPart4Content(slideLayoutPart4);

    slideLayoutPart4.AddPart(slideMasterPart1, "rId1");

    ThemePart themePart1 = slideMasterPart1.AddNewPart<ThemePart>("rId12");
    GenerateThemePart1Content(themePart1);

    SlideLayoutPart slideLayoutPart5 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId2");
    GenerateSlideLayoutPart5Content(slideLayoutPart5);

    slideLayoutPart5.AddPart(slideMasterPart1, "rId1");

    SlideLayoutPart slideLayoutPart6 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId1");
    GenerateSlideLayoutPart6Content(slideLayoutPart6);

    slideLayoutPart6.AddPart(slideMasterPart1, "rId1");

    slideMasterPart1.AddPart(slideLayoutPart1, "rId6");

    SlideLayoutPart slideLayoutPart7 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId11");
    GenerateSlideLayoutPart7Content(slideLayoutPart7);

    slideLayoutPart7.AddPart(slideMasterPart1, "rId1");

    SlideLayoutPart slideLayoutPart8 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId5");
    GenerateSlideLayoutPart8Content(slideLayoutPart8);

    slideLayoutPart8.AddPart(slideMasterPart1, "rId1");

    SlideLayoutPart slideLayoutPart9 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId10");
    GenerateSlideLayoutPart9Content(slideLayoutPart9);

    slideLayoutPart9.AddPart(slideMasterPart1, "rId1");

    SlideLayoutPart slideLayoutPart10 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId4");
    GenerateSlideLayoutPart10Content(slideLayoutPart10);

    slideLayoutPart10.AddPart(slideMasterPart1, "rId1");

    SlideLayoutPart slideLayoutPart11 = slideMasterPart1.AddNewPart<SlideLayoutPart>("rId9");
    GenerateSlideLayoutPart11Content(slideLayoutPart11);

    slideLayoutPart11.AddPart(slideMasterPart1, "rId1");

    presentationPart1.AddPart(themePart1, "rId7");
    presentationPart1.AddPart(slideMasterPart1, "rId1");

    #endregion

    if (bitmaps.Count>1) {
      for (var i = 1; i<bitmaps.Count; i++) {
        SlidePart slidePart = presentationPart1.AddNewPart<SlidePart>("rrId" + (2 + i));
        GenerateSlidePartContent(slidePart, i, bitmaps[i]);
        ImagePart imagePart = slidePart.AddNewPart<ImagePart>("image/jpeg", "rrId" + (2 + i));
        GenerateImagePartContent(imagePart, bitmaps[i].Bitmap);
        slidePart.AddPart(slideLayoutPart1, "rId1");
      }
    }

    ViewPropertiesPart viewPropertiesPart1 = presentationPart1.AddNewPart<ViewPropertiesPart>("rId6");
    GenerateViewPropertiesPart1Content(viewPropertiesPart1);

    PresentationPropertiesPart presentationPropertiesPart1 = presentationPart1.AddNewPart<PresentationPropertiesPart>("rId5");
    GeneratePresentationPropertiesPart1Content(presentationPropertiesPart1);

    ExtendedFilePropertiesPart extendedFilePropertiesPart1 = document.AddNewPart<ExtendedFilePropertiesPart>("rId4");
    GenerateExtendedFilePropertiesPart1Content(extendedFilePropertiesPart1);

    SetPackageProperties(document);
  }

  // Generates content of thumbnailPart1.
  private void GenerateThumbnailPart1Content(ThumbnailPart thumbnailPart1) {
    System.IO.Stream data = GetBinaryDataStream(thumbnailPart1Data);
    thumbnailPart1.FeedData(data);
    data.Close();
  }

  // Generates content of presentationPart1.
  private void GeneratePresentationPart1Content(PresentationPart presentationPart1, List<string> slidesIdList) {
    Presentation presentation1 = new Presentation() {
      SaveSubsetFonts = true
    };
    presentation1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    presentation1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    presentation1.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    SlideMasterIdList slideMasterIdList1 = new SlideMasterIdList();
    SlideMasterId slideMasterId1 = new SlideMasterId() {
      Id = (UInt32Value)2147483648U, RelationshipId = "rId1"
    };

    slideMasterIdList1.Append(slideMasterId1);

    SlideIdList slideIdList1 = new SlideIdList();
    for (var i = 0; i<slidesIdList.Count; i++) {
      SlideId slideId = new SlideId() {
        Id = (256U + (UInt32)i), RelationshipId = slidesIdList[i]
      };
      slideIdList1.Append(slideId);
    }
    SlideSize slideSize1 = new SlideSize() {
      Cx = 9144000, Cy = 6858000, Type = SlideSizeValues.Screen4x3
    };
    NotesSize notesSize1 = new NotesSize() {
      Cx = 6858000L, Cy = 9144000L
    };

    DefaultTextStyle defaultTextStyle1 = new DefaultTextStyle();

    A.DefaultParagraphProperties defaultParagraphProperties1 = new A.DefaultParagraphProperties();
    A.DefaultRunProperties defaultRunProperties1 = new A.DefaultRunProperties() {
      Language = "ru-RU"
    };

    defaultParagraphProperties1.Append(defaultRunProperties1);

    A.Level1ParagraphProperties level1ParagraphProperties1 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties2 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill1 = new A.SolidFill();
    A.SchemeColor schemeColor1 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill1.Append(schemeColor1);
    A.LatinFont latinFont1 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont1 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont1 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties2.Append(solidFill1);
    defaultRunProperties2.Append(latinFont1);
    defaultRunProperties2.Append(eastAsianFont1);
    defaultRunProperties2.Append(complexScriptFont1);

    level1ParagraphProperties1.Append(defaultRunProperties2);

    A.Level2ParagraphProperties level2ParagraphProperties1 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties3 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill2 = new A.SolidFill();
    A.SchemeColor schemeColor2 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill2.Append(schemeColor2);
    A.LatinFont latinFont2 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont2 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont2 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties3.Append(solidFill2);
    defaultRunProperties3.Append(latinFont2);
    defaultRunProperties3.Append(eastAsianFont2);
    defaultRunProperties3.Append(complexScriptFont2);

    level2ParagraphProperties1.Append(defaultRunProperties3);

    A.Level3ParagraphProperties level3ParagraphProperties1 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties4 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill3 = new A.SolidFill();
    A.SchemeColor schemeColor3 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill3.Append(schemeColor3);
    A.LatinFont latinFont3 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont3 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont3 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties4.Append(solidFill3);
    defaultRunProperties4.Append(latinFont3);
    defaultRunProperties4.Append(eastAsianFont3);
    defaultRunProperties4.Append(complexScriptFont3);

    level3ParagraphProperties1.Append(defaultRunProperties4);

    A.Level4ParagraphProperties level4ParagraphProperties1 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties5 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill4 = new A.SolidFill();
    A.SchemeColor schemeColor4 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill4.Append(schemeColor4);
    A.LatinFont latinFont4 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont4 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont4 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties5.Append(solidFill4);
    defaultRunProperties5.Append(latinFont4);
    defaultRunProperties5.Append(eastAsianFont4);
    defaultRunProperties5.Append(complexScriptFont4);

    level4ParagraphProperties1.Append(defaultRunProperties5);

    A.Level5ParagraphProperties level5ParagraphProperties1 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties6 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill5 = new A.SolidFill();
    A.SchemeColor schemeColor5 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill5.Append(schemeColor5);
    A.LatinFont latinFont5 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont5 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont5 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties6.Append(solidFill5);
    defaultRunProperties6.Append(latinFont5);
    defaultRunProperties6.Append(eastAsianFont5);
    defaultRunProperties6.Append(complexScriptFont5);

    level5ParagraphProperties1.Append(defaultRunProperties6);

    A.Level6ParagraphProperties level6ParagraphProperties1 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties7 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill6 = new A.SolidFill();
    A.SchemeColor schemeColor6 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill6.Append(schemeColor6);
    A.LatinFont latinFont6 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont6 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont6 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties7.Append(solidFill6);
    defaultRunProperties7.Append(latinFont6);
    defaultRunProperties7.Append(eastAsianFont6);
    defaultRunProperties7.Append(complexScriptFont6);

    level6ParagraphProperties1.Append(defaultRunProperties7);

    A.Level7ParagraphProperties level7ParagraphProperties1 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties8 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill7 = new A.SolidFill();
    A.SchemeColor schemeColor7 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill7.Append(schemeColor7);
    A.LatinFont latinFont7 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont7 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont7 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties8.Append(solidFill7);
    defaultRunProperties8.Append(latinFont7);
    defaultRunProperties8.Append(eastAsianFont7);
    defaultRunProperties8.Append(complexScriptFont7);

    level7ParagraphProperties1.Append(defaultRunProperties8);

    A.Level8ParagraphProperties level8ParagraphProperties1 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties9 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill8 = new A.SolidFill();
    A.SchemeColor schemeColor8 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill8.Append(schemeColor8);
    A.LatinFont latinFont8 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont8 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont8 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties9.Append(solidFill8);
    defaultRunProperties9.Append(latinFont8);
    defaultRunProperties9.Append(eastAsianFont8);
    defaultRunProperties9.Append(complexScriptFont8);

    level8ParagraphProperties1.Append(defaultRunProperties9);

    A.Level9ParagraphProperties level9ParagraphProperties1 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties10 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill9 = new A.SolidFill();
    A.SchemeColor schemeColor9 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill9.Append(schemeColor9);
    A.LatinFont latinFont9 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont9 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont9 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties10.Append(solidFill9);
    defaultRunProperties10.Append(latinFont9);
    defaultRunProperties10.Append(eastAsianFont9);
    defaultRunProperties10.Append(complexScriptFont9);

    level9ParagraphProperties1.Append(defaultRunProperties10);

    defaultTextStyle1.Append(defaultParagraphProperties1);
    defaultTextStyle1.Append(level1ParagraphProperties1);
    defaultTextStyle1.Append(level2ParagraphProperties1);
    defaultTextStyle1.Append(level3ParagraphProperties1);
    defaultTextStyle1.Append(level4ParagraphProperties1);
    defaultTextStyle1.Append(level5ParagraphProperties1);
    defaultTextStyle1.Append(level6ParagraphProperties1);
    defaultTextStyle1.Append(level7ParagraphProperties1);
    defaultTextStyle1.Append(level8ParagraphProperties1);
    defaultTextStyle1.Append(level9ParagraphProperties1);

    presentation1.Append(slideMasterIdList1);
    presentation1.Append(slideIdList1);
    presentation1.Append(slideSize1);
    presentation1.Append(notesSize1);
    presentation1.Append(defaultTextStyle1);

    presentationPart1.Presentation = presentation1;
  }

  // Generates content of tableStylesPart1.
  private void GenerateTableStylesPart1Content(TableStylesPart tableStylesPart1) {
    A.TableStyleList tableStyleList1 = new A.TableStyleList() {
      Default = "{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}"
    };
    tableStyleList1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

    tableStylesPart1.TableStyleList = tableStyleList1;
  }

  // Generates content of slidePart1.
  private void GenerateSlidePart1Content(SlidePart slidePart1) {
    Slide slide1 = new Slide();
    slide1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slide1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slide1.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData1 = new CommonSlideData();

    ShapeTree shapeTree1 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties1 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties1 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties1 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties1 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties1.Append(nonVisualDrawingProperties1);
    nonVisualGroupShapeProperties1.Append(nonVisualGroupShapeDrawingProperties1);
    nonVisualGroupShapeProperties1.Append(applicationNonVisualDrawingProperties1);

    GroupShapeProperties groupShapeProperties1 = new GroupShapeProperties();

    A.TransformGroup transformGroup1 = new A.TransformGroup();
    A.Offset offset1 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents1 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset1 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents1 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup1.Append(offset1);
    transformGroup1.Append(extents1);
    transformGroup1.Append(childOffset1);
    transformGroup1.Append(childExtents1);

    groupShapeProperties1.Append(transformGroup1);

    Shape shape1 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties1 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties2 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Title 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties1 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks1 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties1.Append(shapeLocks1);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties2 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape1 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties2.Append(placeholderShape1);

    nonVisualShapeProperties1.Append(nonVisualDrawingProperties2);
    nonVisualShapeProperties1.Append(nonVisualShapeDrawingProperties1);
    nonVisualShapeProperties1.Append(applicationNonVisualDrawingProperties2);

    ShapeProperties shapeProperties1 = new ShapeProperties();

    A.Transform2D transform2D1 = new A.Transform2D();
    A.Offset offset2 = new A.Offset() {
      X = 457200L, Y = 274638L
    };
    A.Extents extents2 = new A.Extents() {
      Cx = 8229600L, Cy = 296842L
    };

    transform2D1.Append(offset2);
    transform2D1.Append(extents2);

    shapeProperties1.Append(transform2D1);

    TextBody textBody1 = new TextBody();

    A.BodyProperties bodyProperties1 = new A.BodyProperties();
    A.NoAutoFit noAutoFit1 = new A.NoAutoFit();

    bodyProperties1.Append(noAutoFit1);
    A.ListStyle listStyle1 = new A.ListStyle();

    A.Paragraph paragraph1 = new A.Paragraph();

    A.Run run1 = new A.Run();

    A.RunProperties runProperties1 = new A.RunProperties() {
      Language = "en-US", FontSize = 1400, Dirty = false
    };
    runProperties1.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text1 = new A.Text();
    text1.Text = "Sample title TTT 2";

    run1.Append(runProperties1);
    run1.Append(text1);
    A.EndParagraphRunProperties endParagraphRunProperties1 = new A.EndParagraphRunProperties() {
      Language = "ru-RU", FontSize = 1400, Dirty = false
    };

    paragraph1.Append(run1);
    paragraph1.Append(endParagraphRunProperties1);

    textBody1.Append(bodyProperties1);
    textBody1.Append(listStyle1);
    textBody1.Append(paragraph1);

    shape1.Append(nonVisualShapeProperties1);
    shape1.Append(shapeProperties1);
    shape1.Append(textBody1);

    Picture picture1 = new Picture();

    NonVisualPictureProperties nonVisualPictureProperties1 = new NonVisualPictureProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties3 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Picture 3", Description = "98445rm_22_498.jpg"
    };

    NonVisualPictureDrawingProperties nonVisualPictureDrawingProperties1 = new NonVisualPictureDrawingProperties();
    A.PictureLocks pictureLocks1 = new A.PictureLocks();

    nonVisualPictureDrawingProperties1.Append(pictureLocks1);
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties3 = new ApplicationNonVisualDrawingProperties();

    nonVisualPictureProperties1.Append(nonVisualDrawingProperties3);
    nonVisualPictureProperties1.Append(nonVisualPictureDrawingProperties1);
    nonVisualPictureProperties1.Append(applicationNonVisualDrawingProperties3);

    BlipFill blipFill1 = new BlipFill();
    A.Blip blip1 = new A.Blip() {
      Embed = "rId2"
    };

    A.Stretch stretch1 = new A.Stretch();
    A.FillRectangle fillRectangle1 = new A.FillRectangle();

    stretch1.Append(fillRectangle1);

    blipFill1.Append(blip1);
    blipFill1.Append(stretch1);

    ShapeProperties shapeProperties2 = new ShapeProperties();

    A.Transform2D transform2D2 = new A.Transform2D();
    A.Offset offset3 = new A.Offset() {
      X = 523835L, Y = 690567L
    };
    A.Extents extents3 = new A.Extents() {
      Cx = 8096250L, Cy = 5476875L
    };

    transform2D2.Append(offset3);
    transform2D2.Append(extents3);

    A.PresetGeometry presetGeometry1 = new A.PresetGeometry() {
      Preset = A.ShapeTypeValues.Rectangle
    };
    A.AdjustValueList adjustValueList1 = new A.AdjustValueList();

    presetGeometry1.Append(adjustValueList1);

    shapeProperties2.Append(transform2D2);
    shapeProperties2.Append(presetGeometry1);

    picture1.Append(nonVisualPictureProperties1);
    picture1.Append(blipFill1);
    picture1.Append(shapeProperties2);

    shapeTree1.Append(nonVisualGroupShapeProperties1);
    shapeTree1.Append(groupShapeProperties1);
    shapeTree1.Append(shape1);
    shapeTree1.Append(picture1);

    commonSlideData1.Append(shapeTree1);

    ColorMapOverride colorMapOverride1 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping1 = new A.MasterColorMapping();

    colorMapOverride1.Append(masterColorMapping1);

    slide1.Append(commonSlideData1);
    slide1.Append(colorMapOverride1);

    slidePart1.Slide = slide1;
  }

  // Generates content of slideLayoutPart1.
  private void GenerateSlideLayoutPart1Content(SlideLayoutPart slideLayoutPart1) {
    SlideLayout slideLayout1 = new SlideLayout() {
      Type = SlideLayoutValues.TitleOnly, Preserve = true
    };
    slideLayout1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout1.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData2 = new CommonSlideData() {
      Name = "Title Only"
    };

    ShapeTree shapeTree2 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties2 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties4 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties2 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties4 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties2.Append(nonVisualDrawingProperties4);
    nonVisualGroupShapeProperties2.Append(nonVisualGroupShapeDrawingProperties2);
    nonVisualGroupShapeProperties2.Append(applicationNonVisualDrawingProperties4);

    GroupShapeProperties groupShapeProperties2 = new GroupShapeProperties();

    A.TransformGroup transformGroup2 = new A.TransformGroup();
    A.Offset offset4 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents4 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset2 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents2 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup2.Append(offset4);
    transformGroup2.Append(extents4);
    transformGroup2.Append(childOffset2);
    transformGroup2.Append(childExtents2);

    groupShapeProperties2.Append(transformGroup2);

    Shape shape2 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties2 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties5 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties2 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks2 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties2.Append(shapeLocks2);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties5 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape2 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties5.Append(placeholderShape2);

    nonVisualShapeProperties2.Append(nonVisualDrawingProperties5);
    nonVisualShapeProperties2.Append(nonVisualShapeDrawingProperties2);
    nonVisualShapeProperties2.Append(applicationNonVisualDrawingProperties5);
    ShapeProperties shapeProperties3 = new ShapeProperties();

    TextBody textBody2 = new TextBody();
    A.BodyProperties bodyProperties2 = new A.BodyProperties();
    A.ListStyle listStyle2 = new A.ListStyle();

    A.Paragraph paragraph2 = new A.Paragraph();

    A.Run run2 = new A.Run();

    A.RunProperties runProperties2 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties2.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text2 = new A.Text();
    text2.Text = "Click to edit Master title style";

    run2.Append(runProperties2);
    run2.Append(text2);
    A.EndParagraphRunProperties endParagraphRunProperties2 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph2.Append(run2);
    paragraph2.Append(endParagraphRunProperties2);

    textBody2.Append(bodyProperties2);
    textBody2.Append(listStyle2);
    textBody2.Append(paragraph2);

    shape2.Append(nonVisualShapeProperties2);
    shape2.Append(shapeProperties3);
    shape2.Append(textBody2);

    Shape shape3 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties3 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties6 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Date Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties3 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks3 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties3.Append(shapeLocks3);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties6 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape3 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties6.Append(placeholderShape3);

    nonVisualShapeProperties3.Append(nonVisualDrawingProperties6);
    nonVisualShapeProperties3.Append(nonVisualShapeDrawingProperties3);
    nonVisualShapeProperties3.Append(applicationNonVisualDrawingProperties6);
    ShapeProperties shapeProperties4 = new ShapeProperties();

    TextBody textBody3 = new TextBody();
    A.BodyProperties bodyProperties3 = new A.BodyProperties();
    A.ListStyle listStyle3 = new A.ListStyle();

    A.Paragraph paragraph3 = new A.Paragraph();

    A.Field field1 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties3 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties3.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties1 = new A.ParagraphProperties();
    A.Text text3 = new A.Text();
    text3.Text = "20.11.2013";

    field1.Append(runProperties3);
    field1.Append(paragraphProperties1);
    field1.Append(text3);
    A.EndParagraphRunProperties endParagraphRunProperties3 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph3.Append(field1);
    paragraph3.Append(endParagraphRunProperties3);

    textBody3.Append(bodyProperties3);
    textBody3.Append(listStyle3);
    textBody3.Append(paragraph3);

    shape3.Append(nonVisualShapeProperties3);
    shape3.Append(shapeProperties4);
    shape3.Append(textBody3);

    Shape shape4 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties4 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties7 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Footer Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties4 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks4 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties4.Append(shapeLocks4);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties7 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape4 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties7.Append(placeholderShape4);

    nonVisualShapeProperties4.Append(nonVisualDrawingProperties7);
    nonVisualShapeProperties4.Append(nonVisualShapeDrawingProperties4);
    nonVisualShapeProperties4.Append(applicationNonVisualDrawingProperties7);
    ShapeProperties shapeProperties5 = new ShapeProperties();

    TextBody textBody4 = new TextBody();
    A.BodyProperties bodyProperties4 = new A.BodyProperties();
    A.ListStyle listStyle4 = new A.ListStyle();

    A.Paragraph paragraph4 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties4 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph4.Append(endParagraphRunProperties4);

    textBody4.Append(bodyProperties4);
    textBody4.Append(listStyle4);
    textBody4.Append(paragraph4);

    shape4.Append(nonVisualShapeProperties4);
    shape4.Append(shapeProperties5);
    shape4.Append(textBody4);

    Shape shape5 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties5 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties8 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Slide Number Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties5 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks5 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties5.Append(shapeLocks5);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties8 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape5 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties8.Append(placeholderShape5);

    nonVisualShapeProperties5.Append(nonVisualDrawingProperties8);
    nonVisualShapeProperties5.Append(nonVisualShapeDrawingProperties5);
    nonVisualShapeProperties5.Append(applicationNonVisualDrawingProperties8);
    ShapeProperties shapeProperties6 = new ShapeProperties();

    TextBody textBody5 = new TextBody();
    A.BodyProperties bodyProperties5 = new A.BodyProperties();
    A.ListStyle listStyle5 = new A.ListStyle();

    A.Paragraph paragraph5 = new A.Paragraph();

    A.Field field2 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties4 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties4.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties2 = new A.ParagraphProperties();
    A.Text text4 = new A.Text();
    text4.Text = "‹#›";

    field2.Append(runProperties4);
    field2.Append(paragraphProperties2);
    field2.Append(text4);
    A.EndParagraphRunProperties endParagraphRunProperties5 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph5.Append(field2);
    paragraph5.Append(endParagraphRunProperties5);

    textBody5.Append(bodyProperties5);
    textBody5.Append(listStyle5);
    textBody5.Append(paragraph5);

    shape5.Append(nonVisualShapeProperties5);
    shape5.Append(shapeProperties6);
    shape5.Append(textBody5);

    shapeTree2.Append(nonVisualGroupShapeProperties2);
    shapeTree2.Append(groupShapeProperties2);
    shapeTree2.Append(shape2);
    shapeTree2.Append(shape3);
    shapeTree2.Append(shape4);
    shapeTree2.Append(shape5);

    commonSlideData2.Append(shapeTree2);

    ColorMapOverride colorMapOverride2 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping2 = new A.MasterColorMapping();

    colorMapOverride2.Append(masterColorMapping2);

    slideLayout1.Append(commonSlideData2);
    slideLayout1.Append(colorMapOverride2);

    slideLayoutPart1.SlideLayout = slideLayout1;
  }

  // Generates content of slideMasterPart1.
  private void GenerateSlideMasterPart1Content(SlideMasterPart slideMasterPart1) {
    SlideMaster slideMaster1 = new SlideMaster();
    slideMaster1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideMaster1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideMaster1.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData3 = new CommonSlideData();

    Background background1 = new Background();

    BackgroundStyleReference backgroundStyleReference1 = new BackgroundStyleReference() {
      Index = (UInt32Value)1001U
    };
    A.SchemeColor schemeColor10 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Background1
    };

    backgroundStyleReference1.Append(schemeColor10);

    background1.Append(backgroundStyleReference1);

    ShapeTree shapeTree3 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties3 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties9 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties3 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties9 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties3.Append(nonVisualDrawingProperties9);
    nonVisualGroupShapeProperties3.Append(nonVisualGroupShapeDrawingProperties3);
    nonVisualGroupShapeProperties3.Append(applicationNonVisualDrawingProperties9);

    GroupShapeProperties groupShapeProperties3 = new GroupShapeProperties();

    A.TransformGroup transformGroup3 = new A.TransformGroup();
    A.Offset offset5 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents5 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset3 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents3 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup3.Append(offset5);
    transformGroup3.Append(extents5);
    transformGroup3.Append(childOffset3);
    transformGroup3.Append(childExtents3);

    groupShapeProperties3.Append(transformGroup3);

    Shape shape6 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties6 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties10 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title Placeholder 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties6 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks6 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties6.Append(shapeLocks6);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties10 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape6 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties10.Append(placeholderShape6);

    nonVisualShapeProperties6.Append(nonVisualDrawingProperties10);
    nonVisualShapeProperties6.Append(nonVisualShapeDrawingProperties6);
    nonVisualShapeProperties6.Append(applicationNonVisualDrawingProperties10);

    ShapeProperties shapeProperties7 = new ShapeProperties();

    A.Transform2D transform2D3 = new A.Transform2D();
    A.Offset offset6 = new A.Offset() {
      X = 457200L, Y = 274638L
    };
    A.Extents extents6 = new A.Extents() {
      Cx = 8229600L, Cy = 1143000L
    };

    transform2D3.Append(offset6);
    transform2D3.Append(extents6);

    A.PresetGeometry presetGeometry2 = new A.PresetGeometry() {
      Preset = A.ShapeTypeValues.Rectangle
    };
    A.AdjustValueList adjustValueList2 = new A.AdjustValueList();

    presetGeometry2.Append(adjustValueList2);

    shapeProperties7.Append(transform2D3);
    shapeProperties7.Append(presetGeometry2);

    TextBody textBody6 = new TextBody();

    A.BodyProperties bodyProperties6 = new A.BodyProperties() {
      Vertical = A.TextVerticalValues.Horizontal, LeftInset = 91440, TopInset = 45720, RightInset = 91440, BottomInset = 45720, RightToLeftColumns = false, Anchor = A.TextAnchoringTypeValues.Center
    };
    A.NormalAutoFit normalAutoFit1 = new A.NormalAutoFit();

    bodyProperties6.Append(normalAutoFit1);
    A.ListStyle listStyle6 = new A.ListStyle();

    A.Paragraph paragraph6 = new A.Paragraph();

    A.Run run3 = new A.Run();

    A.RunProperties runProperties5 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties5.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text5 = new A.Text();
    text5.Text = "Click to edit Master title style";

    run3.Append(runProperties5);
    run3.Append(text5);
    A.EndParagraphRunProperties endParagraphRunProperties6 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph6.Append(run3);
    paragraph6.Append(endParagraphRunProperties6);

    textBody6.Append(bodyProperties6);
    textBody6.Append(listStyle6);
    textBody6.Append(paragraph6);

    shape6.Append(nonVisualShapeProperties6);
    shape6.Append(shapeProperties7);
    shape6.Append(textBody6);

    Shape shape7 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties7 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties11 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Text Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties7 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks7 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties7.Append(shapeLocks7);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties11 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape7 = new PlaceholderShape() {
      Type = PlaceholderValues.Body, Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties11.Append(placeholderShape7);

    nonVisualShapeProperties7.Append(nonVisualDrawingProperties11);
    nonVisualShapeProperties7.Append(nonVisualShapeDrawingProperties7);
    nonVisualShapeProperties7.Append(applicationNonVisualDrawingProperties11);

    ShapeProperties shapeProperties8 = new ShapeProperties();

    A.Transform2D transform2D4 = new A.Transform2D();
    A.Offset offset7 = new A.Offset() {
      X = 457200L, Y = 1600200L
    };
    A.Extents extents7 = new A.Extents() {
      Cx = 8229600L, Cy = 4525963L
    };

    transform2D4.Append(offset7);
    transform2D4.Append(extents7);

    A.PresetGeometry presetGeometry3 = new A.PresetGeometry() {
      Preset = A.ShapeTypeValues.Rectangle
    };
    A.AdjustValueList adjustValueList3 = new A.AdjustValueList();

    presetGeometry3.Append(adjustValueList3);

    shapeProperties8.Append(transform2D4);
    shapeProperties8.Append(presetGeometry3);

    TextBody textBody7 = new TextBody();

    A.BodyProperties bodyProperties7 = new A.BodyProperties() {
      Vertical = A.TextVerticalValues.Horizontal, LeftInset = 91440, TopInset = 45720, RightInset = 91440, BottomInset = 45720, RightToLeftColumns = false
    };
    A.NormalAutoFit normalAutoFit2 = new A.NormalAutoFit();

    bodyProperties7.Append(normalAutoFit2);
    A.ListStyle listStyle7 = new A.ListStyle();

    A.Paragraph paragraph7 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties3 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run4 = new A.Run();

    A.RunProperties runProperties6 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties6.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text6 = new A.Text();
    text6.Text = "Click to edit Master text styles";

    run4.Append(runProperties6);
    run4.Append(text6);

    paragraph7.Append(paragraphProperties3);
    paragraph7.Append(run4);

    A.Paragraph paragraph8 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties4 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run5 = new A.Run();

    A.RunProperties runProperties7 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties7.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text7 = new A.Text();
    text7.Text = "Second level";

    run5.Append(runProperties7);
    run5.Append(text7);

    paragraph8.Append(paragraphProperties4);
    paragraph8.Append(run5);

    A.Paragraph paragraph9 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties5 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run6 = new A.Run();

    A.RunProperties runProperties8 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties8.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text8 = new A.Text();
    text8.Text = "Third level";

    run6.Append(runProperties8);
    run6.Append(text8);

    paragraph9.Append(paragraphProperties5);
    paragraph9.Append(run6);

    A.Paragraph paragraph10 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties6 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run7 = new A.Run();

    A.RunProperties runProperties9 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties9.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text9 = new A.Text();
    text9.Text = "Fourth level";

    run7.Append(runProperties9);
    run7.Append(text9);

    paragraph10.Append(paragraphProperties6);
    paragraph10.Append(run7);

    A.Paragraph paragraph11 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties7 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run8 = new A.Run();

    A.RunProperties runProperties10 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties10.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text10 = new A.Text();
    text10.Text = "Fifth level";

    run8.Append(runProperties10);
    run8.Append(text10);
    A.EndParagraphRunProperties endParagraphRunProperties7 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph11.Append(paragraphProperties7);
    paragraph11.Append(run8);
    paragraph11.Append(endParagraphRunProperties7);

    textBody7.Append(bodyProperties7);
    textBody7.Append(listStyle7);
    textBody7.Append(paragraph7);
    textBody7.Append(paragraph8);
    textBody7.Append(paragraph9);
    textBody7.Append(paragraph10);
    textBody7.Append(paragraph11);

    shape7.Append(nonVisualShapeProperties7);
    shape7.Append(shapeProperties8);
    shape7.Append(textBody7);

    Shape shape8 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties8 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties12 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Date Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties8 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks8 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties8.Append(shapeLocks8);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties12 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape8 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)2U
    };

    applicationNonVisualDrawingProperties12.Append(placeholderShape8);

    nonVisualShapeProperties8.Append(nonVisualDrawingProperties12);
    nonVisualShapeProperties8.Append(nonVisualShapeDrawingProperties8);
    nonVisualShapeProperties8.Append(applicationNonVisualDrawingProperties12);

    ShapeProperties shapeProperties9 = new ShapeProperties();

    A.Transform2D transform2D5 = new A.Transform2D();
    A.Offset offset8 = new A.Offset() {
      X = 457200L, Y = 6356350L
    };
    A.Extents extents8 = new A.Extents() {
      Cx = 2133600L, Cy = 365125L
    };

    transform2D5.Append(offset8);
    transform2D5.Append(extents8);

    A.PresetGeometry presetGeometry4 = new A.PresetGeometry() {
      Preset = A.ShapeTypeValues.Rectangle
    };
    A.AdjustValueList adjustValueList4 = new A.AdjustValueList();

    presetGeometry4.Append(adjustValueList4);

    shapeProperties9.Append(transform2D5);
    shapeProperties9.Append(presetGeometry4);

    TextBody textBody8 = new TextBody();
    A.BodyProperties bodyProperties8 = new A.BodyProperties() {
      Vertical = A.TextVerticalValues.Horizontal, LeftInset = 91440, TopInset = 45720, RightInset = 91440, BottomInset = 45720, RightToLeftColumns = false, Anchor = A.TextAnchoringTypeValues.Center
    };

    A.ListStyle listStyle8 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties2 = new A.Level1ParagraphProperties() {
      Alignment = A.TextAlignmentTypeValues.Left
    };

    A.DefaultRunProperties defaultRunProperties11 = new A.DefaultRunProperties() {
      FontSize = 1200
    };

    A.SolidFill solidFill10 = new A.SolidFill();

    A.SchemeColor schemeColor11 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint1 = new A.Tint() {
      Val = 75000
    };

    schemeColor11.Append(tint1);

    solidFill10.Append(schemeColor11);

    defaultRunProperties11.Append(solidFill10);

    level1ParagraphProperties2.Append(defaultRunProperties11);

    listStyle8.Append(level1ParagraphProperties2);

    A.Paragraph paragraph12 = new A.Paragraph();

    A.Field field3 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties11 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties11.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties8 = new A.ParagraphProperties();
    A.Text text11 = new A.Text();
    text11.Text = "20.11.2013";

    field3.Append(runProperties11);
    field3.Append(paragraphProperties8);
    field3.Append(text11);
    A.EndParagraphRunProperties endParagraphRunProperties8 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph12.Append(field3);
    paragraph12.Append(endParagraphRunProperties8);

    textBody8.Append(bodyProperties8);
    textBody8.Append(listStyle8);
    textBody8.Append(paragraph12);

    shape8.Append(nonVisualShapeProperties8);
    shape8.Append(shapeProperties9);
    shape8.Append(textBody8);

    Shape shape9 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties9 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties13 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Footer Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties9 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks9 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties9.Append(shapeLocks9);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties13 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape9 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)3U
    };

    applicationNonVisualDrawingProperties13.Append(placeholderShape9);

    nonVisualShapeProperties9.Append(nonVisualDrawingProperties13);
    nonVisualShapeProperties9.Append(nonVisualShapeDrawingProperties9);
    nonVisualShapeProperties9.Append(applicationNonVisualDrawingProperties13);

    ShapeProperties shapeProperties10 = new ShapeProperties();

    A.Transform2D transform2D6 = new A.Transform2D();
    A.Offset offset9 = new A.Offset() {
      X = 3124200L, Y = 6356350L
    };
    A.Extents extents9 = new A.Extents() {
      Cx = 2895600L, Cy = 365125L
    };

    transform2D6.Append(offset9);
    transform2D6.Append(extents9);

    A.PresetGeometry presetGeometry5 = new A.PresetGeometry() {
      Preset = A.ShapeTypeValues.Rectangle
    };
    A.AdjustValueList adjustValueList5 = new A.AdjustValueList();

    presetGeometry5.Append(adjustValueList5);

    shapeProperties10.Append(transform2D6);
    shapeProperties10.Append(presetGeometry5);

    TextBody textBody9 = new TextBody();
    A.BodyProperties bodyProperties9 = new A.BodyProperties() {
      Vertical = A.TextVerticalValues.Horizontal, LeftInset = 91440, TopInset = 45720, RightInset = 91440, BottomInset = 45720, RightToLeftColumns = false, Anchor = A.TextAnchoringTypeValues.Center
    };

    A.ListStyle listStyle9 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties3 = new A.Level1ParagraphProperties() {
      Alignment = A.TextAlignmentTypeValues.Center
    };

    A.DefaultRunProperties defaultRunProperties12 = new A.DefaultRunProperties() {
      FontSize = 1200
    };

    A.SolidFill solidFill11 = new A.SolidFill();

    A.SchemeColor schemeColor12 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint2 = new A.Tint() {
      Val = 75000
    };

    schemeColor12.Append(tint2);

    solidFill11.Append(schemeColor12);

    defaultRunProperties12.Append(solidFill11);

    level1ParagraphProperties3.Append(defaultRunProperties12);

    listStyle9.Append(level1ParagraphProperties3);

    A.Paragraph paragraph13 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties9 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph13.Append(endParagraphRunProperties9);

    textBody9.Append(bodyProperties9);
    textBody9.Append(listStyle9);
    textBody9.Append(paragraph13);

    shape9.Append(nonVisualShapeProperties9);
    shape9.Append(shapeProperties10);
    shape9.Append(textBody9);

    Shape shape10 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties10 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties14 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Slide Number Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties10 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks10 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties10.Append(shapeLocks10);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties14 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape10 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)4U
    };

    applicationNonVisualDrawingProperties14.Append(placeholderShape10);

    nonVisualShapeProperties10.Append(nonVisualDrawingProperties14);
    nonVisualShapeProperties10.Append(nonVisualShapeDrawingProperties10);
    nonVisualShapeProperties10.Append(applicationNonVisualDrawingProperties14);

    ShapeProperties shapeProperties11 = new ShapeProperties();

    A.Transform2D transform2D7 = new A.Transform2D();
    A.Offset offset10 = new A.Offset() {
      X = 6553200L, Y = 6356350L
    };
    A.Extents extents10 = new A.Extents() {
      Cx = 2133600L, Cy = 365125L
    };

    transform2D7.Append(offset10);
    transform2D7.Append(extents10);

    A.PresetGeometry presetGeometry6 = new A.PresetGeometry() {
      Preset = A.ShapeTypeValues.Rectangle
    };
    A.AdjustValueList adjustValueList6 = new A.AdjustValueList();

    presetGeometry6.Append(adjustValueList6);

    shapeProperties11.Append(transform2D7);
    shapeProperties11.Append(presetGeometry6);

    TextBody textBody10 = new TextBody();
    A.BodyProperties bodyProperties10 = new A.BodyProperties() {
      Vertical = A.TextVerticalValues.Horizontal, LeftInset = 91440, TopInset = 45720, RightInset = 91440, BottomInset = 45720, RightToLeftColumns = false, Anchor = A.TextAnchoringTypeValues.Center
    };

    A.ListStyle listStyle10 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties4 = new A.Level1ParagraphProperties() {
      Alignment = A.TextAlignmentTypeValues.Right
    };

    A.DefaultRunProperties defaultRunProperties13 = new A.DefaultRunProperties() {
      FontSize = 1200
    };

    A.SolidFill solidFill12 = new A.SolidFill();

    A.SchemeColor schemeColor13 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint3 = new A.Tint() {
      Val = 75000
    };

    schemeColor13.Append(tint3);

    solidFill12.Append(schemeColor13);

    defaultRunProperties13.Append(solidFill12);

    level1ParagraphProperties4.Append(defaultRunProperties13);

    listStyle10.Append(level1ParagraphProperties4);

    A.Paragraph paragraph14 = new A.Paragraph();

    A.Field field4 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties12 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties12.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties9 = new A.ParagraphProperties();
    A.Text text12 = new A.Text();
    text12.Text = "‹#›";

    field4.Append(runProperties12);
    field4.Append(paragraphProperties9);
    field4.Append(text12);
    A.EndParagraphRunProperties endParagraphRunProperties10 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph14.Append(field4);
    paragraph14.Append(endParagraphRunProperties10);

    textBody10.Append(bodyProperties10);
    textBody10.Append(listStyle10);
    textBody10.Append(paragraph14);

    shape10.Append(nonVisualShapeProperties10);
    shape10.Append(shapeProperties11);
    shape10.Append(textBody10);

    shapeTree3.Append(nonVisualGroupShapeProperties3);
    shapeTree3.Append(groupShapeProperties3);
    shapeTree3.Append(shape6);
    shapeTree3.Append(shape7);
    shapeTree3.Append(shape8);
    shapeTree3.Append(shape9);
    shapeTree3.Append(shape10);

    commonSlideData3.Append(background1);
    commonSlideData3.Append(shapeTree3);
    ColorMap colorMap1 = new ColorMap() {
      Background1 = A.ColorSchemeIndexValues.Light1, Text1 = A.ColorSchemeIndexValues.Dark1, Background2 = A.ColorSchemeIndexValues.Light2, Text2 = A.ColorSchemeIndexValues.Dark2, Accent1 = A.ColorSchemeIndexValues.Accent1, Accent2 = A.ColorSchemeIndexValues.Accent2, Accent3 = A.ColorSchemeIndexValues.Accent3, Accent4 = A.ColorSchemeIndexValues.Accent4, Accent5 = A.ColorSchemeIndexValues.Accent5, Accent6 = A.ColorSchemeIndexValues.Accent6, Hyperlink = A.ColorSchemeIndexValues.Hyperlink, FollowedHyperlink = A.ColorSchemeIndexValues.FollowedHyperlink
    };

    SlideLayoutIdList slideLayoutIdList1 = new SlideLayoutIdList();
    SlideLayoutId slideLayoutId1 = new SlideLayoutId() {
      Id = (UInt32Value)2147483649U, RelationshipId = "rId1"
    };
    SlideLayoutId slideLayoutId2 = new SlideLayoutId() {
      Id = (UInt32Value)2147483650U, RelationshipId = "rId2"
    };
    SlideLayoutId slideLayoutId3 = new SlideLayoutId() {
      Id = (UInt32Value)2147483651U, RelationshipId = "rId3"
    };
    SlideLayoutId slideLayoutId4 = new SlideLayoutId() {
      Id = (UInt32Value)2147483652U, RelationshipId = "rId4"
    };
    SlideLayoutId slideLayoutId5 = new SlideLayoutId() {
      Id = (UInt32Value)2147483653U, RelationshipId = "rId5"
    };
    SlideLayoutId slideLayoutId6 = new SlideLayoutId() {
      Id = (UInt32Value)2147483654U, RelationshipId = "rId6"
    };
    SlideLayoutId slideLayoutId7 = new SlideLayoutId() {
      Id = (UInt32Value)2147483655U, RelationshipId = "rId7"
    };
    SlideLayoutId slideLayoutId8 = new SlideLayoutId() {
      Id = (UInt32Value)2147483656U, RelationshipId = "rId8"
    };
    SlideLayoutId slideLayoutId9 = new SlideLayoutId() {
      Id = (UInt32Value)2147483657U, RelationshipId = "rId9"
    };
    SlideLayoutId slideLayoutId10 = new SlideLayoutId() {
      Id = (UInt32Value)2147483658U, RelationshipId = "rId10"
    };
    SlideLayoutId slideLayoutId11 = new SlideLayoutId() {
      Id = (UInt32Value)2147483659U, RelationshipId = "rId11"
    };

    slideLayoutIdList1.Append(slideLayoutId1);
    slideLayoutIdList1.Append(slideLayoutId2);
    slideLayoutIdList1.Append(slideLayoutId3);
    slideLayoutIdList1.Append(slideLayoutId4);
    slideLayoutIdList1.Append(slideLayoutId5);
    slideLayoutIdList1.Append(slideLayoutId6);
    slideLayoutIdList1.Append(slideLayoutId7);
    slideLayoutIdList1.Append(slideLayoutId8);
    slideLayoutIdList1.Append(slideLayoutId9);
    slideLayoutIdList1.Append(slideLayoutId10);
    slideLayoutIdList1.Append(slideLayoutId11);

    TextStyles textStyles1 = new TextStyles();

    TitleStyle titleStyle1 = new TitleStyle();

    A.Level1ParagraphProperties level1ParagraphProperties5 = new A.Level1ParagraphProperties() {
      Alignment = A.TextAlignmentTypeValues.Center, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore1 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent1 = new A.SpacingPercent() {
      Val = 0
    };

    spaceBefore1.Append(spacingPercent1);
    A.NoBullet noBullet1 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties14 = new A.DefaultRunProperties() {
      FontSize = 4400, Kerning = 1200
    };

    A.SolidFill solidFill13 = new A.SolidFill();
    A.SchemeColor schemeColor14 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill13.Append(schemeColor14);
    A.LatinFont latinFont10 = new A.LatinFont() {
      Typeface = "+mj-lt"
    };
    A.EastAsianFont eastAsianFont10 = new A.EastAsianFont() {
      Typeface = "+mj-ea"
    };
    A.ComplexScriptFont complexScriptFont10 = new A.ComplexScriptFont() {
      Typeface = "+mj-cs"
    };

    defaultRunProperties14.Append(solidFill13);
    defaultRunProperties14.Append(latinFont10);
    defaultRunProperties14.Append(eastAsianFont10);
    defaultRunProperties14.Append(complexScriptFont10);

    level1ParagraphProperties5.Append(spaceBefore1);
    level1ParagraphProperties5.Append(noBullet1);
    level1ParagraphProperties5.Append(defaultRunProperties14);

    titleStyle1.Append(level1ParagraphProperties5);

    BodyStyle bodyStyle1 = new BodyStyle();

    A.Level1ParagraphProperties level1ParagraphProperties6 = new A.Level1ParagraphProperties() {
      LeftMargin = 342900, Indent = -342900, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore2 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent2 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore2.Append(spacingPercent2);
    A.BulletFont bulletFont1 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet1 = new A.CharacterBullet() {
      Char = "•"
    };

    A.DefaultRunProperties defaultRunProperties15 = new A.DefaultRunProperties() {
      FontSize = 3200, Kerning = 1200
    };

    A.SolidFill solidFill14 = new A.SolidFill();
    A.SchemeColor schemeColor15 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill14.Append(schemeColor15);
    A.LatinFont latinFont11 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont11 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont11 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties15.Append(solidFill14);
    defaultRunProperties15.Append(latinFont11);
    defaultRunProperties15.Append(eastAsianFont11);
    defaultRunProperties15.Append(complexScriptFont11);

    level1ParagraphProperties6.Append(spaceBefore2);
    level1ParagraphProperties6.Append(bulletFont1);
    level1ParagraphProperties6.Append(characterBullet1);
    level1ParagraphProperties6.Append(defaultRunProperties15);

    A.Level2ParagraphProperties level2ParagraphProperties2 = new A.Level2ParagraphProperties() {
      LeftMargin = 742950, Indent = -285750, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore3 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent3 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore3.Append(spacingPercent3);
    A.BulletFont bulletFont2 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet2 = new A.CharacterBullet() {
      Char = "–"
    };

    A.DefaultRunProperties defaultRunProperties16 = new A.DefaultRunProperties() {
      FontSize = 2800, Kerning = 1200
    };

    A.SolidFill solidFill15 = new A.SolidFill();
    A.SchemeColor schemeColor16 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill15.Append(schemeColor16);
    A.LatinFont latinFont12 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont12 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont12 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties16.Append(solidFill15);
    defaultRunProperties16.Append(latinFont12);
    defaultRunProperties16.Append(eastAsianFont12);
    defaultRunProperties16.Append(complexScriptFont12);

    level2ParagraphProperties2.Append(spaceBefore3);
    level2ParagraphProperties2.Append(bulletFont2);
    level2ParagraphProperties2.Append(characterBullet2);
    level2ParagraphProperties2.Append(defaultRunProperties16);

    A.Level3ParagraphProperties level3ParagraphProperties2 = new A.Level3ParagraphProperties() {
      LeftMargin = 1143000, Indent = -228600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore4 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent4 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore4.Append(spacingPercent4);
    A.BulletFont bulletFont3 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet3 = new A.CharacterBullet() {
      Char = "•"
    };

    A.DefaultRunProperties defaultRunProperties17 = new A.DefaultRunProperties() {
      FontSize = 2400, Kerning = 1200
    };

    A.SolidFill solidFill16 = new A.SolidFill();
    A.SchemeColor schemeColor17 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill16.Append(schemeColor17);
    A.LatinFont latinFont13 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont13 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont13 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties17.Append(solidFill16);
    defaultRunProperties17.Append(latinFont13);
    defaultRunProperties17.Append(eastAsianFont13);
    defaultRunProperties17.Append(complexScriptFont13);

    level3ParagraphProperties2.Append(spaceBefore4);
    level3ParagraphProperties2.Append(bulletFont3);
    level3ParagraphProperties2.Append(characterBullet3);
    level3ParagraphProperties2.Append(defaultRunProperties17);

    A.Level4ParagraphProperties level4ParagraphProperties2 = new A.Level4ParagraphProperties() {
      LeftMargin = 1600200, Indent = -228600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore5 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent5 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore5.Append(spacingPercent5);
    A.BulletFont bulletFont4 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet4 = new A.CharacterBullet() {
      Char = "–"
    };

    A.DefaultRunProperties defaultRunProperties18 = new A.DefaultRunProperties() {
      FontSize = 2000, Kerning = 1200
    };

    A.SolidFill solidFill17 = new A.SolidFill();
    A.SchemeColor schemeColor18 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill17.Append(schemeColor18);
    A.LatinFont latinFont14 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont14 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont14 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties18.Append(solidFill17);
    defaultRunProperties18.Append(latinFont14);
    defaultRunProperties18.Append(eastAsianFont14);
    defaultRunProperties18.Append(complexScriptFont14);

    level4ParagraphProperties2.Append(spaceBefore5);
    level4ParagraphProperties2.Append(bulletFont4);
    level4ParagraphProperties2.Append(characterBullet4);
    level4ParagraphProperties2.Append(defaultRunProperties18);

    A.Level5ParagraphProperties level5ParagraphProperties2 = new A.Level5ParagraphProperties() {
      LeftMargin = 2057400, Indent = -228600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore6 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent6 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore6.Append(spacingPercent6);
    A.BulletFont bulletFont5 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet5 = new A.CharacterBullet() {
      Char = "»"
    };

    A.DefaultRunProperties defaultRunProperties19 = new A.DefaultRunProperties() {
      FontSize = 2000, Kerning = 1200
    };

    A.SolidFill solidFill18 = new A.SolidFill();
    A.SchemeColor schemeColor19 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill18.Append(schemeColor19);
    A.LatinFont latinFont15 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont15 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont15 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties19.Append(solidFill18);
    defaultRunProperties19.Append(latinFont15);
    defaultRunProperties19.Append(eastAsianFont15);
    defaultRunProperties19.Append(complexScriptFont15);

    level5ParagraphProperties2.Append(spaceBefore6);
    level5ParagraphProperties2.Append(bulletFont5);
    level5ParagraphProperties2.Append(characterBullet5);
    level5ParagraphProperties2.Append(defaultRunProperties19);

    A.Level6ParagraphProperties level6ParagraphProperties2 = new A.Level6ParagraphProperties() {
      LeftMargin = 2514600, Indent = -228600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore7 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent7 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore7.Append(spacingPercent7);
    A.BulletFont bulletFont6 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet6 = new A.CharacterBullet() {
      Char = "•"
    };

    A.DefaultRunProperties defaultRunProperties20 = new A.DefaultRunProperties() {
      FontSize = 2000, Kerning = 1200
    };

    A.SolidFill solidFill19 = new A.SolidFill();
    A.SchemeColor schemeColor20 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill19.Append(schemeColor20);
    A.LatinFont latinFont16 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont16 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont16 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties20.Append(solidFill19);
    defaultRunProperties20.Append(latinFont16);
    defaultRunProperties20.Append(eastAsianFont16);
    defaultRunProperties20.Append(complexScriptFont16);

    level6ParagraphProperties2.Append(spaceBefore7);
    level6ParagraphProperties2.Append(bulletFont6);
    level6ParagraphProperties2.Append(characterBullet6);
    level6ParagraphProperties2.Append(defaultRunProperties20);

    A.Level7ParagraphProperties level7ParagraphProperties2 = new A.Level7ParagraphProperties() {
      LeftMargin = 2971800, Indent = -228600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore8 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent8 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore8.Append(spacingPercent8);
    A.BulletFont bulletFont7 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet7 = new A.CharacterBullet() {
      Char = "•"
    };

    A.DefaultRunProperties defaultRunProperties21 = new A.DefaultRunProperties() {
      FontSize = 2000, Kerning = 1200
    };

    A.SolidFill solidFill20 = new A.SolidFill();
    A.SchemeColor schemeColor21 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill20.Append(schemeColor21);
    A.LatinFont latinFont17 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont17 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont17 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties21.Append(solidFill20);
    defaultRunProperties21.Append(latinFont17);
    defaultRunProperties21.Append(eastAsianFont17);
    defaultRunProperties21.Append(complexScriptFont17);

    level7ParagraphProperties2.Append(spaceBefore8);
    level7ParagraphProperties2.Append(bulletFont7);
    level7ParagraphProperties2.Append(characterBullet7);
    level7ParagraphProperties2.Append(defaultRunProperties21);

    A.Level8ParagraphProperties level8ParagraphProperties2 = new A.Level8ParagraphProperties() {
      LeftMargin = 3429000, Indent = -228600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore9 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent9 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore9.Append(spacingPercent9);
    A.BulletFont bulletFont8 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet8 = new A.CharacterBullet() {
      Char = "•"
    };

    A.DefaultRunProperties defaultRunProperties22 = new A.DefaultRunProperties() {
      FontSize = 2000, Kerning = 1200
    };

    A.SolidFill solidFill21 = new A.SolidFill();
    A.SchemeColor schemeColor22 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill21.Append(schemeColor22);
    A.LatinFont latinFont18 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont18 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont18 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties22.Append(solidFill21);
    defaultRunProperties22.Append(latinFont18);
    defaultRunProperties22.Append(eastAsianFont18);
    defaultRunProperties22.Append(complexScriptFont18);

    level8ParagraphProperties2.Append(spaceBefore9);
    level8ParagraphProperties2.Append(bulletFont8);
    level8ParagraphProperties2.Append(characterBullet8);
    level8ParagraphProperties2.Append(defaultRunProperties22);

    A.Level9ParagraphProperties level9ParagraphProperties2 = new A.Level9ParagraphProperties() {
      LeftMargin = 3886200, Indent = -228600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.SpaceBefore spaceBefore10 = new A.SpaceBefore();
    A.SpacingPercent spacingPercent10 = new A.SpacingPercent() {
      Val = 20000
    };

    spaceBefore10.Append(spacingPercent10);
    A.BulletFont bulletFont9 = new A.BulletFont() {
      Typeface = "Arial", PitchFamily = 34, CharacterSet = 0
    };
    A.CharacterBullet characterBullet9 = new A.CharacterBullet() {
      Char = "•"
    };

    A.DefaultRunProperties defaultRunProperties23 = new A.DefaultRunProperties() {
      FontSize = 2000, Kerning = 1200
    };

    A.SolidFill solidFill22 = new A.SolidFill();
    A.SchemeColor schemeColor23 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill22.Append(schemeColor23);
    A.LatinFont latinFont19 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont19 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont19 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties23.Append(solidFill22);
    defaultRunProperties23.Append(latinFont19);
    defaultRunProperties23.Append(eastAsianFont19);
    defaultRunProperties23.Append(complexScriptFont19);

    level9ParagraphProperties2.Append(spaceBefore10);
    level9ParagraphProperties2.Append(bulletFont9);
    level9ParagraphProperties2.Append(characterBullet9);
    level9ParagraphProperties2.Append(defaultRunProperties23);

    bodyStyle1.Append(level1ParagraphProperties6);
    bodyStyle1.Append(level2ParagraphProperties2);
    bodyStyle1.Append(level3ParagraphProperties2);
    bodyStyle1.Append(level4ParagraphProperties2);
    bodyStyle1.Append(level5ParagraphProperties2);
    bodyStyle1.Append(level6ParagraphProperties2);
    bodyStyle1.Append(level7ParagraphProperties2);
    bodyStyle1.Append(level8ParagraphProperties2);
    bodyStyle1.Append(level9ParagraphProperties2);

    OtherStyle otherStyle1 = new OtherStyle();

    A.DefaultParagraphProperties defaultParagraphProperties2 = new A.DefaultParagraphProperties();
    A.DefaultRunProperties defaultRunProperties24 = new A.DefaultRunProperties() {
      Language = "ru-RU"
    };

    defaultParagraphProperties2.Append(defaultRunProperties24);

    A.Level1ParagraphProperties level1ParagraphProperties7 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties25 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill23 = new A.SolidFill();
    A.SchemeColor schemeColor24 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill23.Append(schemeColor24);
    A.LatinFont latinFont20 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont20 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont20 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties25.Append(solidFill23);
    defaultRunProperties25.Append(latinFont20);
    defaultRunProperties25.Append(eastAsianFont20);
    defaultRunProperties25.Append(complexScriptFont20);

    level1ParagraphProperties7.Append(defaultRunProperties25);

    A.Level2ParagraphProperties level2ParagraphProperties3 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties26 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill24 = new A.SolidFill();
    A.SchemeColor schemeColor25 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill24.Append(schemeColor25);
    A.LatinFont latinFont21 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont21 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont21 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties26.Append(solidFill24);
    defaultRunProperties26.Append(latinFont21);
    defaultRunProperties26.Append(eastAsianFont21);
    defaultRunProperties26.Append(complexScriptFont21);

    level2ParagraphProperties3.Append(defaultRunProperties26);

    A.Level3ParagraphProperties level3ParagraphProperties3 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties27 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill25 = new A.SolidFill();
    A.SchemeColor schemeColor26 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill25.Append(schemeColor26);
    A.LatinFont latinFont22 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont22 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont22 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties27.Append(solidFill25);
    defaultRunProperties27.Append(latinFont22);
    defaultRunProperties27.Append(eastAsianFont22);
    defaultRunProperties27.Append(complexScriptFont22);

    level3ParagraphProperties3.Append(defaultRunProperties27);

    A.Level4ParagraphProperties level4ParagraphProperties3 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties28 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill26 = new A.SolidFill();
    A.SchemeColor schemeColor27 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill26.Append(schemeColor27);
    A.LatinFont latinFont23 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont23 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont23 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties28.Append(solidFill26);
    defaultRunProperties28.Append(latinFont23);
    defaultRunProperties28.Append(eastAsianFont23);
    defaultRunProperties28.Append(complexScriptFont23);

    level4ParagraphProperties3.Append(defaultRunProperties28);

    A.Level5ParagraphProperties level5ParagraphProperties3 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties29 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill27 = new A.SolidFill();
    A.SchemeColor schemeColor28 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill27.Append(schemeColor28);
    A.LatinFont latinFont24 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont24 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont24 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties29.Append(solidFill27);
    defaultRunProperties29.Append(latinFont24);
    defaultRunProperties29.Append(eastAsianFont24);
    defaultRunProperties29.Append(complexScriptFont24);

    level5ParagraphProperties3.Append(defaultRunProperties29);

    A.Level6ParagraphProperties level6ParagraphProperties3 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties30 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill28 = new A.SolidFill();
    A.SchemeColor schemeColor29 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill28.Append(schemeColor29);
    A.LatinFont latinFont25 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont25 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont25 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties30.Append(solidFill28);
    defaultRunProperties30.Append(latinFont25);
    defaultRunProperties30.Append(eastAsianFont25);
    defaultRunProperties30.Append(complexScriptFont25);

    level6ParagraphProperties3.Append(defaultRunProperties30);

    A.Level7ParagraphProperties level7ParagraphProperties3 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties31 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill29 = new A.SolidFill();
    A.SchemeColor schemeColor30 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill29.Append(schemeColor30);
    A.LatinFont latinFont26 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont26 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont26 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties31.Append(solidFill29);
    defaultRunProperties31.Append(latinFont26);
    defaultRunProperties31.Append(eastAsianFont26);
    defaultRunProperties31.Append(complexScriptFont26);

    level7ParagraphProperties3.Append(defaultRunProperties31);

    A.Level8ParagraphProperties level8ParagraphProperties3 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties32 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill30 = new A.SolidFill();
    A.SchemeColor schemeColor31 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill30.Append(schemeColor31);
    A.LatinFont latinFont27 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont27 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont27 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties32.Append(solidFill30);
    defaultRunProperties32.Append(latinFont27);
    defaultRunProperties32.Append(eastAsianFont27);
    defaultRunProperties32.Append(complexScriptFont27);

    level8ParagraphProperties3.Append(defaultRunProperties32);

    A.Level9ParagraphProperties level9ParagraphProperties3 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Alignment = A.TextAlignmentTypeValues.Left, DefaultTabSize = 914400, RightToLeft = false, EastAsianLineBreak = true, LatinLineBreak = false, Height = true
    };

    A.DefaultRunProperties defaultRunProperties33 = new A.DefaultRunProperties() {
      FontSize = 1800, Kerning = 1200
    };

    A.SolidFill solidFill31 = new A.SolidFill();
    A.SchemeColor schemeColor32 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };

    solidFill31.Append(schemeColor32);
    A.LatinFont latinFont28 = new A.LatinFont() {
      Typeface = "+mn-lt"
    };
    A.EastAsianFont eastAsianFont28 = new A.EastAsianFont() {
      Typeface = "+mn-ea"
    };
    A.ComplexScriptFont complexScriptFont28 = new A.ComplexScriptFont() {
      Typeface = "+mn-cs"
    };

    defaultRunProperties33.Append(solidFill31);
    defaultRunProperties33.Append(latinFont28);
    defaultRunProperties33.Append(eastAsianFont28);
    defaultRunProperties33.Append(complexScriptFont28);

    level9ParagraphProperties3.Append(defaultRunProperties33);

    otherStyle1.Append(defaultParagraphProperties2);
    otherStyle1.Append(level1ParagraphProperties7);
    otherStyle1.Append(level2ParagraphProperties3);
    otherStyle1.Append(level3ParagraphProperties3);
    otherStyle1.Append(level4ParagraphProperties3);
    otherStyle1.Append(level5ParagraphProperties3);
    otherStyle1.Append(level6ParagraphProperties3);
    otherStyle1.Append(level7ParagraphProperties3);
    otherStyle1.Append(level8ParagraphProperties3);
    otherStyle1.Append(level9ParagraphProperties3);

    textStyles1.Append(titleStyle1);
    textStyles1.Append(bodyStyle1);
    textStyles1.Append(otherStyle1);

    slideMaster1.Append(commonSlideData3);
    slideMaster1.Append(colorMap1);
    slideMaster1.Append(slideLayoutIdList1);
    slideMaster1.Append(textStyles1);

    slideMasterPart1.SlideMaster = slideMaster1;
  }

  // Generates content of slideLayoutPart2.
  private void GenerateSlideLayoutPart2Content(SlideLayoutPart slideLayoutPart2) {
    SlideLayout slideLayout2 = new SlideLayout() {
      Type = SlideLayoutValues.ObjectText, Preserve = true
    };
    slideLayout2.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout2.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout2.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData4 = new CommonSlideData() {
      Name = "Content with Caption"
    };

    ShapeTree shapeTree4 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties4 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties15 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties4 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties15 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties4.Append(nonVisualDrawingProperties15);
    nonVisualGroupShapeProperties4.Append(nonVisualGroupShapeDrawingProperties4);
    nonVisualGroupShapeProperties4.Append(applicationNonVisualDrawingProperties15);

    GroupShapeProperties groupShapeProperties4 = new GroupShapeProperties();

    A.TransformGroup transformGroup4 = new A.TransformGroup();
    A.Offset offset11 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents11 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset4 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents4 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup4.Append(offset11);
    transformGroup4.Append(extents11);
    transformGroup4.Append(childOffset4);
    transformGroup4.Append(childExtents4);

    groupShapeProperties4.Append(transformGroup4);

    Shape shape11 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties11 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties16 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties11 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks11 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties11.Append(shapeLocks11);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties16 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape11 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties16.Append(placeholderShape11);

    nonVisualShapeProperties11.Append(nonVisualDrawingProperties16);
    nonVisualShapeProperties11.Append(nonVisualShapeDrawingProperties11);
    nonVisualShapeProperties11.Append(applicationNonVisualDrawingProperties16);

    ShapeProperties shapeProperties12 = new ShapeProperties();

    A.Transform2D transform2D8 = new A.Transform2D();
    A.Offset offset12 = new A.Offset() {
      X = 457200L, Y = 273050L
    };
    A.Extents extents12 = new A.Extents() {
      Cx = 3008313L, Cy = 1162050L
    };

    transform2D8.Append(offset12);
    transform2D8.Append(extents12);

    shapeProperties12.Append(transform2D8);

    TextBody textBody11 = new TextBody();
    A.BodyProperties bodyProperties11 = new A.BodyProperties() {
      Anchor = A.TextAnchoringTypeValues.Bottom
    };

    A.ListStyle listStyle11 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties8 = new A.Level1ParagraphProperties() {
      Alignment = A.TextAlignmentTypeValues.Left
    };
    A.DefaultRunProperties defaultRunProperties34 = new A.DefaultRunProperties() {
      FontSize = 2000, Bold = true
    };

    level1ParagraphProperties8.Append(defaultRunProperties34);

    listStyle11.Append(level1ParagraphProperties8);

    A.Paragraph paragraph15 = new A.Paragraph();

    A.Run run9 = new A.Run();

    A.RunProperties runProperties13 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties13.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text13 = new A.Text();
    text13.Text = "Click to edit Master title style";

    run9.Append(runProperties13);
    run9.Append(text13);
    A.EndParagraphRunProperties endParagraphRunProperties11 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph15.Append(run9);
    paragraph15.Append(endParagraphRunProperties11);

    textBody11.Append(bodyProperties11);
    textBody11.Append(listStyle11);
    textBody11.Append(paragraph15);

    shape11.Append(nonVisualShapeProperties11);
    shape11.Append(shapeProperties12);
    shape11.Append(textBody11);

    Shape shape12 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties12 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties17 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Content Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties12 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks12 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties12.Append(shapeLocks12);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties17 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape12 = new PlaceholderShape() {
      Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties17.Append(placeholderShape12);

    nonVisualShapeProperties12.Append(nonVisualDrawingProperties17);
    nonVisualShapeProperties12.Append(nonVisualShapeDrawingProperties12);
    nonVisualShapeProperties12.Append(applicationNonVisualDrawingProperties17);

    ShapeProperties shapeProperties13 = new ShapeProperties();

    A.Transform2D transform2D9 = new A.Transform2D();
    A.Offset offset13 = new A.Offset() {
      X = 3575050L, Y = 273050L
    };
    A.Extents extents13 = new A.Extents() {
      Cx = 5111750L, Cy = 5853113L
    };

    transform2D9.Append(offset13);
    transform2D9.Append(extents13);

    shapeProperties13.Append(transform2D9);

    TextBody textBody12 = new TextBody();
    A.BodyProperties bodyProperties12 = new A.BodyProperties();

    A.ListStyle listStyle12 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties9 = new A.Level1ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties35 = new A.DefaultRunProperties() {
      FontSize = 3200
    };

    level1ParagraphProperties9.Append(defaultRunProperties35);

    A.Level2ParagraphProperties level2ParagraphProperties4 = new A.Level2ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties36 = new A.DefaultRunProperties() {
      FontSize = 2800
    };

    level2ParagraphProperties4.Append(defaultRunProperties36);

    A.Level3ParagraphProperties level3ParagraphProperties4 = new A.Level3ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties37 = new A.DefaultRunProperties() {
      FontSize = 2400
    };

    level3ParagraphProperties4.Append(defaultRunProperties37);

    A.Level4ParagraphProperties level4ParagraphProperties4 = new A.Level4ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties38 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level4ParagraphProperties4.Append(defaultRunProperties38);

    A.Level5ParagraphProperties level5ParagraphProperties4 = new A.Level5ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties39 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level5ParagraphProperties4.Append(defaultRunProperties39);

    A.Level6ParagraphProperties level6ParagraphProperties4 = new A.Level6ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties40 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level6ParagraphProperties4.Append(defaultRunProperties40);

    A.Level7ParagraphProperties level7ParagraphProperties4 = new A.Level7ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties41 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level7ParagraphProperties4.Append(defaultRunProperties41);

    A.Level8ParagraphProperties level8ParagraphProperties4 = new A.Level8ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties42 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level8ParagraphProperties4.Append(defaultRunProperties42);

    A.Level9ParagraphProperties level9ParagraphProperties4 = new A.Level9ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties43 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level9ParagraphProperties4.Append(defaultRunProperties43);

    listStyle12.Append(level1ParagraphProperties9);
    listStyle12.Append(level2ParagraphProperties4);
    listStyle12.Append(level3ParagraphProperties4);
    listStyle12.Append(level4ParagraphProperties4);
    listStyle12.Append(level5ParagraphProperties4);
    listStyle12.Append(level6ParagraphProperties4);
    listStyle12.Append(level7ParagraphProperties4);
    listStyle12.Append(level8ParagraphProperties4);
    listStyle12.Append(level9ParagraphProperties4);

    A.Paragraph paragraph16 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties10 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run10 = new A.Run();

    A.RunProperties runProperties14 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties14.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text14 = new A.Text();
    text14.Text = "Click to edit Master text styles";

    run10.Append(runProperties14);
    run10.Append(text14);

    paragraph16.Append(paragraphProperties10);
    paragraph16.Append(run10);

    A.Paragraph paragraph17 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties11 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run11 = new A.Run();

    A.RunProperties runProperties15 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties15.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text15 = new A.Text();
    text15.Text = "Second level";

    run11.Append(runProperties15);
    run11.Append(text15);

    paragraph17.Append(paragraphProperties11);
    paragraph17.Append(run11);

    A.Paragraph paragraph18 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties12 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run12 = new A.Run();

    A.RunProperties runProperties16 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties16.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text16 = new A.Text();
    text16.Text = "Third level";

    run12.Append(runProperties16);
    run12.Append(text16);

    paragraph18.Append(paragraphProperties12);
    paragraph18.Append(run12);

    A.Paragraph paragraph19 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties13 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run13 = new A.Run();

    A.RunProperties runProperties17 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties17.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text17 = new A.Text();
    text17.Text = "Fourth level";

    run13.Append(runProperties17);
    run13.Append(text17);

    paragraph19.Append(paragraphProperties13);
    paragraph19.Append(run13);

    A.Paragraph paragraph20 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties14 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run14 = new A.Run();

    A.RunProperties runProperties18 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties18.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text18 = new A.Text();
    text18.Text = "Fifth level";

    run14.Append(runProperties18);
    run14.Append(text18);
    A.EndParagraphRunProperties endParagraphRunProperties12 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph20.Append(paragraphProperties14);
    paragraph20.Append(run14);
    paragraph20.Append(endParagraphRunProperties12);

    textBody12.Append(bodyProperties12);
    textBody12.Append(listStyle12);
    textBody12.Append(paragraph16);
    textBody12.Append(paragraph17);
    textBody12.Append(paragraph18);
    textBody12.Append(paragraph19);
    textBody12.Append(paragraph20);

    shape12.Append(nonVisualShapeProperties12);
    shape12.Append(shapeProperties13);
    shape12.Append(textBody12);

    Shape shape13 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties13 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties18 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Text Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties13 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks13 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties13.Append(shapeLocks13);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties18 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape13 = new PlaceholderShape() {
      Type = PlaceholderValues.Body, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)2U
    };

    applicationNonVisualDrawingProperties18.Append(placeholderShape13);

    nonVisualShapeProperties13.Append(nonVisualDrawingProperties18);
    nonVisualShapeProperties13.Append(nonVisualShapeDrawingProperties13);
    nonVisualShapeProperties13.Append(applicationNonVisualDrawingProperties18);

    ShapeProperties shapeProperties14 = new ShapeProperties();

    A.Transform2D transform2D10 = new A.Transform2D();
    A.Offset offset14 = new A.Offset() {
      X = 457200L, Y = 1435100L
    };
    A.Extents extents14 = new A.Extents() {
      Cx = 3008313L, Cy = 4691063L
    };

    transform2D10.Append(offset14);
    transform2D10.Append(extents14);

    shapeProperties14.Append(transform2D10);

    TextBody textBody13 = new TextBody();
    A.BodyProperties bodyProperties13 = new A.BodyProperties();

    A.ListStyle listStyle13 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties10 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Indent = 0
    };
    A.NoBullet noBullet2 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties44 = new A.DefaultRunProperties() {
      FontSize = 1400
    };

    level1ParagraphProperties10.Append(noBullet2);
    level1ParagraphProperties10.Append(defaultRunProperties44);

    A.Level2ParagraphProperties level2ParagraphProperties5 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Indent = 0
    };
    A.NoBullet noBullet3 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties45 = new A.DefaultRunProperties() {
      FontSize = 1200
    };

    level2ParagraphProperties5.Append(noBullet3);
    level2ParagraphProperties5.Append(defaultRunProperties45);

    A.Level3ParagraphProperties level3ParagraphProperties5 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Indent = 0
    };
    A.NoBullet noBullet4 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties46 = new A.DefaultRunProperties() {
      FontSize = 1000
    };

    level3ParagraphProperties5.Append(noBullet4);
    level3ParagraphProperties5.Append(defaultRunProperties46);

    A.Level4ParagraphProperties level4ParagraphProperties5 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Indent = 0
    };
    A.NoBullet noBullet5 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties47 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level4ParagraphProperties5.Append(noBullet5);
    level4ParagraphProperties5.Append(defaultRunProperties47);

    A.Level5ParagraphProperties level5ParagraphProperties5 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Indent = 0
    };
    A.NoBullet noBullet6 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties48 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level5ParagraphProperties5.Append(noBullet6);
    level5ParagraphProperties5.Append(defaultRunProperties48);

    A.Level6ParagraphProperties level6ParagraphProperties5 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Indent = 0
    };
    A.NoBullet noBullet7 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties49 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level6ParagraphProperties5.Append(noBullet7);
    level6ParagraphProperties5.Append(defaultRunProperties49);

    A.Level7ParagraphProperties level7ParagraphProperties5 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Indent = 0
    };
    A.NoBullet noBullet8 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties50 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level7ParagraphProperties5.Append(noBullet8);
    level7ParagraphProperties5.Append(defaultRunProperties50);

    A.Level8ParagraphProperties level8ParagraphProperties5 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Indent = 0
    };
    A.NoBullet noBullet9 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties51 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level8ParagraphProperties5.Append(noBullet9);
    level8ParagraphProperties5.Append(defaultRunProperties51);

    A.Level9ParagraphProperties level9ParagraphProperties5 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Indent = 0
    };
    A.NoBullet noBullet10 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties52 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level9ParagraphProperties5.Append(noBullet10);
    level9ParagraphProperties5.Append(defaultRunProperties52);

    listStyle13.Append(level1ParagraphProperties10);
    listStyle13.Append(level2ParagraphProperties5);
    listStyle13.Append(level3ParagraphProperties5);
    listStyle13.Append(level4ParagraphProperties5);
    listStyle13.Append(level5ParagraphProperties5);
    listStyle13.Append(level6ParagraphProperties5);
    listStyle13.Append(level7ParagraphProperties5);
    listStyle13.Append(level8ParagraphProperties5);
    listStyle13.Append(level9ParagraphProperties5);

    A.Paragraph paragraph21 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties15 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run15 = new A.Run();

    A.RunProperties runProperties19 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties19.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text19 = new A.Text();
    text19.Text = "Click to edit Master text styles";

    run15.Append(runProperties19);
    run15.Append(text19);

    paragraph21.Append(paragraphProperties15);
    paragraph21.Append(run15);

    textBody13.Append(bodyProperties13);
    textBody13.Append(listStyle13);
    textBody13.Append(paragraph21);

    shape13.Append(nonVisualShapeProperties13);
    shape13.Append(shapeProperties14);
    shape13.Append(textBody13);

    Shape shape14 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties14 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties19 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Date Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties14 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks14 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties14.Append(shapeLocks14);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties19 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape14 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties19.Append(placeholderShape14);

    nonVisualShapeProperties14.Append(nonVisualDrawingProperties19);
    nonVisualShapeProperties14.Append(nonVisualShapeDrawingProperties14);
    nonVisualShapeProperties14.Append(applicationNonVisualDrawingProperties19);
    ShapeProperties shapeProperties15 = new ShapeProperties();

    TextBody textBody14 = new TextBody();
    A.BodyProperties bodyProperties14 = new A.BodyProperties();
    A.ListStyle listStyle14 = new A.ListStyle();

    A.Paragraph paragraph22 = new A.Paragraph();

    A.Field field5 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties20 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties20.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties16 = new A.ParagraphProperties();
    A.Text text20 = new A.Text();
    text20.Text = "20.11.2013";

    field5.Append(runProperties20);
    field5.Append(paragraphProperties16);
    field5.Append(text20);
    A.EndParagraphRunProperties endParagraphRunProperties13 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph22.Append(field5);
    paragraph22.Append(endParagraphRunProperties13);

    textBody14.Append(bodyProperties14);
    textBody14.Append(listStyle14);
    textBody14.Append(paragraph22);

    shape14.Append(nonVisualShapeProperties14);
    shape14.Append(shapeProperties15);
    shape14.Append(textBody14);

    Shape shape15 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties15 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties20 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Footer Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties15 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks15 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties15.Append(shapeLocks15);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties20 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape15 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties20.Append(placeholderShape15);

    nonVisualShapeProperties15.Append(nonVisualDrawingProperties20);
    nonVisualShapeProperties15.Append(nonVisualShapeDrawingProperties15);
    nonVisualShapeProperties15.Append(applicationNonVisualDrawingProperties20);
    ShapeProperties shapeProperties16 = new ShapeProperties();

    TextBody textBody15 = new TextBody();
    A.BodyProperties bodyProperties15 = new A.BodyProperties();
    A.ListStyle listStyle15 = new A.ListStyle();

    A.Paragraph paragraph23 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties14 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph23.Append(endParagraphRunProperties14);

    textBody15.Append(bodyProperties15);
    textBody15.Append(listStyle15);
    textBody15.Append(paragraph23);

    shape15.Append(nonVisualShapeProperties15);
    shape15.Append(shapeProperties16);
    shape15.Append(textBody15);

    Shape shape16 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties16 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties21 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)7U, Name = "Slide Number Placeholder 6"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties16 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks16 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties16.Append(shapeLocks16);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties21 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape16 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties21.Append(placeholderShape16);

    nonVisualShapeProperties16.Append(nonVisualDrawingProperties21);
    nonVisualShapeProperties16.Append(nonVisualShapeDrawingProperties16);
    nonVisualShapeProperties16.Append(applicationNonVisualDrawingProperties21);
    ShapeProperties shapeProperties17 = new ShapeProperties();

    TextBody textBody16 = new TextBody();
    A.BodyProperties bodyProperties16 = new A.BodyProperties();
    A.ListStyle listStyle16 = new A.ListStyle();

    A.Paragraph paragraph24 = new A.Paragraph();

    A.Field field6 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties21 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties21.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties17 = new A.ParagraphProperties();
    A.Text text21 = new A.Text();
    text21.Text = "‹#›";

    field6.Append(runProperties21);
    field6.Append(paragraphProperties17);
    field6.Append(text21);
    A.EndParagraphRunProperties endParagraphRunProperties15 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph24.Append(field6);
    paragraph24.Append(endParagraphRunProperties15);

    textBody16.Append(bodyProperties16);
    textBody16.Append(listStyle16);
    textBody16.Append(paragraph24);

    shape16.Append(nonVisualShapeProperties16);
    shape16.Append(shapeProperties17);
    shape16.Append(textBody16);

    shapeTree4.Append(nonVisualGroupShapeProperties4);
    shapeTree4.Append(groupShapeProperties4);
    shapeTree4.Append(shape11);
    shapeTree4.Append(shape12);
    shapeTree4.Append(shape13);
    shapeTree4.Append(shape14);
    shapeTree4.Append(shape15);
    shapeTree4.Append(shape16);

    commonSlideData4.Append(shapeTree4);

    ColorMapOverride colorMapOverride3 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping3 = new A.MasterColorMapping();

    colorMapOverride3.Append(masterColorMapping3);

    slideLayout2.Append(commonSlideData4);
    slideLayout2.Append(colorMapOverride3);

    slideLayoutPart2.SlideLayout = slideLayout2;
  }

  // Generates content of slideLayoutPart3.
  private void GenerateSlideLayoutPart3Content(SlideLayoutPart slideLayoutPart3) {
    SlideLayout slideLayout3 = new SlideLayout() {
      Type = SlideLayoutValues.SectionHeader, Preserve = true
    };
    slideLayout3.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout3.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout3.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData5 = new CommonSlideData() {
      Name = "Section Header"
    };

    ShapeTree shapeTree5 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties5 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties22 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties5 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties22 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties5.Append(nonVisualDrawingProperties22);
    nonVisualGroupShapeProperties5.Append(nonVisualGroupShapeDrawingProperties5);
    nonVisualGroupShapeProperties5.Append(applicationNonVisualDrawingProperties22);

    GroupShapeProperties groupShapeProperties5 = new GroupShapeProperties();

    A.TransformGroup transformGroup5 = new A.TransformGroup();
    A.Offset offset15 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents15 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset5 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents5 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup5.Append(offset15);
    transformGroup5.Append(extents15);
    transformGroup5.Append(childOffset5);
    transformGroup5.Append(childExtents5);

    groupShapeProperties5.Append(transformGroup5);

    Shape shape17 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties17 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties23 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties17 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks17 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties17.Append(shapeLocks17);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties23 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape17 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties23.Append(placeholderShape17);

    nonVisualShapeProperties17.Append(nonVisualDrawingProperties23);
    nonVisualShapeProperties17.Append(nonVisualShapeDrawingProperties17);
    nonVisualShapeProperties17.Append(applicationNonVisualDrawingProperties23);

    ShapeProperties shapeProperties18 = new ShapeProperties();

    A.Transform2D transform2D11 = new A.Transform2D();
    A.Offset offset16 = new A.Offset() {
      X = 722313L, Y = 4406900L
    };
    A.Extents extents16 = new A.Extents() {
      Cx = 7772400L, Cy = 1362075L
    };

    transform2D11.Append(offset16);
    transform2D11.Append(extents16);

    shapeProperties18.Append(transform2D11);

    TextBody textBody17 = new TextBody();
    A.BodyProperties bodyProperties17 = new A.BodyProperties() {
      Anchor = A.TextAnchoringTypeValues.Top
    };

    A.ListStyle listStyle17 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties11 = new A.Level1ParagraphProperties() {
      Alignment = A.TextAlignmentTypeValues.Left
    };
    A.DefaultRunProperties defaultRunProperties53 = new A.DefaultRunProperties() {
      FontSize = 4000, Bold = true, Capital = A.TextCapsValues.All
    };

    level1ParagraphProperties11.Append(defaultRunProperties53);

    listStyle17.Append(level1ParagraphProperties11);

    A.Paragraph paragraph25 = new A.Paragraph();

    A.Run run16 = new A.Run();

    A.RunProperties runProperties22 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties22.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text22 = new A.Text();
    text22.Text = "Click to edit Master title style";

    run16.Append(runProperties22);
    run16.Append(text22);
    A.EndParagraphRunProperties endParagraphRunProperties16 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph25.Append(run16);
    paragraph25.Append(endParagraphRunProperties16);

    textBody17.Append(bodyProperties17);
    textBody17.Append(listStyle17);
    textBody17.Append(paragraph25);

    shape17.Append(nonVisualShapeProperties17);
    shape17.Append(shapeProperties18);
    shape17.Append(textBody17);

    Shape shape18 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties18 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties24 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Text Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties18 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks18 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties18.Append(shapeLocks18);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties24 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape18 = new PlaceholderShape() {
      Type = PlaceholderValues.Body, Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties24.Append(placeholderShape18);

    nonVisualShapeProperties18.Append(nonVisualDrawingProperties24);
    nonVisualShapeProperties18.Append(nonVisualShapeDrawingProperties18);
    nonVisualShapeProperties18.Append(applicationNonVisualDrawingProperties24);

    ShapeProperties shapeProperties19 = new ShapeProperties();

    A.Transform2D transform2D12 = new A.Transform2D();
    A.Offset offset17 = new A.Offset() {
      X = 722313L, Y = 2906713L
    };
    A.Extents extents17 = new A.Extents() {
      Cx = 7772400L, Cy = 1500187L
    };

    transform2D12.Append(offset17);
    transform2D12.Append(extents17);

    shapeProperties19.Append(transform2D12);

    TextBody textBody18 = new TextBody();
    A.BodyProperties bodyProperties18 = new A.BodyProperties() {
      Anchor = A.TextAnchoringTypeValues.Bottom
    };

    A.ListStyle listStyle18 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties12 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Indent = 0
    };
    A.NoBullet noBullet11 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties54 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    A.SolidFill solidFill32 = new A.SolidFill();

    A.SchemeColor schemeColor33 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint4 = new A.Tint() {
      Val = 75000
    };

    schemeColor33.Append(tint4);

    solidFill32.Append(schemeColor33);

    defaultRunProperties54.Append(solidFill32);

    level1ParagraphProperties12.Append(noBullet11);
    level1ParagraphProperties12.Append(defaultRunProperties54);

    A.Level2ParagraphProperties level2ParagraphProperties6 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Indent = 0
    };
    A.NoBullet noBullet12 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties55 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    A.SolidFill solidFill33 = new A.SolidFill();

    A.SchemeColor schemeColor34 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint5 = new A.Tint() {
      Val = 75000
    };

    schemeColor34.Append(tint5);

    solidFill33.Append(schemeColor34);

    defaultRunProperties55.Append(solidFill33);

    level2ParagraphProperties6.Append(noBullet12);
    level2ParagraphProperties6.Append(defaultRunProperties55);

    A.Level3ParagraphProperties level3ParagraphProperties6 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Indent = 0
    };
    A.NoBullet noBullet13 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties56 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    A.SolidFill solidFill34 = new A.SolidFill();

    A.SchemeColor schemeColor35 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint6 = new A.Tint() {
      Val = 75000
    };

    schemeColor35.Append(tint6);

    solidFill34.Append(schemeColor35);

    defaultRunProperties56.Append(solidFill34);

    level3ParagraphProperties6.Append(noBullet13);
    level3ParagraphProperties6.Append(defaultRunProperties56);

    A.Level4ParagraphProperties level4ParagraphProperties6 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Indent = 0
    };
    A.NoBullet noBullet14 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties57 = new A.DefaultRunProperties() {
      FontSize = 1400
    };

    A.SolidFill solidFill35 = new A.SolidFill();

    A.SchemeColor schemeColor36 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint7 = new A.Tint() {
      Val = 75000
    };

    schemeColor36.Append(tint7);

    solidFill35.Append(schemeColor36);

    defaultRunProperties57.Append(solidFill35);

    level4ParagraphProperties6.Append(noBullet14);
    level4ParagraphProperties6.Append(defaultRunProperties57);

    A.Level5ParagraphProperties level5ParagraphProperties6 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Indent = 0
    };
    A.NoBullet noBullet15 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties58 = new A.DefaultRunProperties() {
      FontSize = 1400
    };

    A.SolidFill solidFill36 = new A.SolidFill();

    A.SchemeColor schemeColor37 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint8 = new A.Tint() {
      Val = 75000
    };

    schemeColor37.Append(tint8);

    solidFill36.Append(schemeColor37);

    defaultRunProperties58.Append(solidFill36);

    level5ParagraphProperties6.Append(noBullet15);
    level5ParagraphProperties6.Append(defaultRunProperties58);

    A.Level6ParagraphProperties level6ParagraphProperties6 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Indent = 0
    };
    A.NoBullet noBullet16 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties59 = new A.DefaultRunProperties() {
      FontSize = 1400
    };

    A.SolidFill solidFill37 = new A.SolidFill();

    A.SchemeColor schemeColor38 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint9 = new A.Tint() {
      Val = 75000
    };

    schemeColor38.Append(tint9);

    solidFill37.Append(schemeColor38);

    defaultRunProperties59.Append(solidFill37);

    level6ParagraphProperties6.Append(noBullet16);
    level6ParagraphProperties6.Append(defaultRunProperties59);

    A.Level7ParagraphProperties level7ParagraphProperties6 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Indent = 0
    };
    A.NoBullet noBullet17 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties60 = new A.DefaultRunProperties() {
      FontSize = 1400
    };

    A.SolidFill solidFill38 = new A.SolidFill();

    A.SchemeColor schemeColor39 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint10 = new A.Tint() {
      Val = 75000
    };

    schemeColor39.Append(tint10);

    solidFill38.Append(schemeColor39);

    defaultRunProperties60.Append(solidFill38);

    level7ParagraphProperties6.Append(noBullet17);
    level7ParagraphProperties6.Append(defaultRunProperties60);

    A.Level8ParagraphProperties level8ParagraphProperties6 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Indent = 0
    };
    A.NoBullet noBullet18 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties61 = new A.DefaultRunProperties() {
      FontSize = 1400
    };

    A.SolidFill solidFill39 = new A.SolidFill();

    A.SchemeColor schemeColor40 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint11 = new A.Tint() {
      Val = 75000
    };

    schemeColor40.Append(tint11);

    solidFill39.Append(schemeColor40);

    defaultRunProperties61.Append(solidFill39);

    level8ParagraphProperties6.Append(noBullet18);
    level8ParagraphProperties6.Append(defaultRunProperties61);

    A.Level9ParagraphProperties level9ParagraphProperties6 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Indent = 0
    };
    A.NoBullet noBullet19 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties62 = new A.DefaultRunProperties() {
      FontSize = 1400
    };

    A.SolidFill solidFill40 = new A.SolidFill();

    A.SchemeColor schemeColor41 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint12 = new A.Tint() {
      Val = 75000
    };

    schemeColor41.Append(tint12);

    solidFill40.Append(schemeColor41);

    defaultRunProperties62.Append(solidFill40);

    level9ParagraphProperties6.Append(noBullet19);
    level9ParagraphProperties6.Append(defaultRunProperties62);

    listStyle18.Append(level1ParagraphProperties12);
    listStyle18.Append(level2ParagraphProperties6);
    listStyle18.Append(level3ParagraphProperties6);
    listStyle18.Append(level4ParagraphProperties6);
    listStyle18.Append(level5ParagraphProperties6);
    listStyle18.Append(level6ParagraphProperties6);
    listStyle18.Append(level7ParagraphProperties6);
    listStyle18.Append(level8ParagraphProperties6);
    listStyle18.Append(level9ParagraphProperties6);

    A.Paragraph paragraph26 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties18 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run17 = new A.Run();

    A.RunProperties runProperties23 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties23.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text23 = new A.Text();
    text23.Text = "Click to edit Master text styles";

    run17.Append(runProperties23);
    run17.Append(text23);

    paragraph26.Append(paragraphProperties18);
    paragraph26.Append(run17);

    textBody18.Append(bodyProperties18);
    textBody18.Append(listStyle18);
    textBody18.Append(paragraph26);

    shape18.Append(nonVisualShapeProperties18);
    shape18.Append(shapeProperties19);
    shape18.Append(textBody18);

    Shape shape19 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties19 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties25 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Date Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties19 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks19 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties19.Append(shapeLocks19);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties25 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape19 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties25.Append(placeholderShape19);

    nonVisualShapeProperties19.Append(nonVisualDrawingProperties25);
    nonVisualShapeProperties19.Append(nonVisualShapeDrawingProperties19);
    nonVisualShapeProperties19.Append(applicationNonVisualDrawingProperties25);
    ShapeProperties shapeProperties20 = new ShapeProperties();

    TextBody textBody19 = new TextBody();
    A.BodyProperties bodyProperties19 = new A.BodyProperties();
    A.ListStyle listStyle19 = new A.ListStyle();

    A.Paragraph paragraph27 = new A.Paragraph();

    A.Field field7 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties24 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties24.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties19 = new A.ParagraphProperties();
    A.Text text24 = new A.Text();
    text24.Text = "20.11.2013";

    field7.Append(runProperties24);
    field7.Append(paragraphProperties19);
    field7.Append(text24);
    A.EndParagraphRunProperties endParagraphRunProperties17 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph27.Append(field7);
    paragraph27.Append(endParagraphRunProperties17);

    textBody19.Append(bodyProperties19);
    textBody19.Append(listStyle19);
    textBody19.Append(paragraph27);

    shape19.Append(nonVisualShapeProperties19);
    shape19.Append(shapeProperties20);
    shape19.Append(textBody19);

    Shape shape20 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties20 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties26 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Footer Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties20 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks20 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties20.Append(shapeLocks20);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties26 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape20 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties26.Append(placeholderShape20);

    nonVisualShapeProperties20.Append(nonVisualDrawingProperties26);
    nonVisualShapeProperties20.Append(nonVisualShapeDrawingProperties20);
    nonVisualShapeProperties20.Append(applicationNonVisualDrawingProperties26);
    ShapeProperties shapeProperties21 = new ShapeProperties();

    TextBody textBody20 = new TextBody();
    A.BodyProperties bodyProperties20 = new A.BodyProperties();
    A.ListStyle listStyle20 = new A.ListStyle();

    A.Paragraph paragraph28 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties18 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph28.Append(endParagraphRunProperties18);

    textBody20.Append(bodyProperties20);
    textBody20.Append(listStyle20);
    textBody20.Append(paragraph28);

    shape20.Append(nonVisualShapeProperties20);
    shape20.Append(shapeProperties21);
    shape20.Append(textBody20);

    Shape shape21 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties21 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties27 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Slide Number Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties21 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks21 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties21.Append(shapeLocks21);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties27 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape21 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties27.Append(placeholderShape21);

    nonVisualShapeProperties21.Append(nonVisualDrawingProperties27);
    nonVisualShapeProperties21.Append(nonVisualShapeDrawingProperties21);
    nonVisualShapeProperties21.Append(applicationNonVisualDrawingProperties27);
    ShapeProperties shapeProperties22 = new ShapeProperties();

    TextBody textBody21 = new TextBody();
    A.BodyProperties bodyProperties21 = new A.BodyProperties();
    A.ListStyle listStyle21 = new A.ListStyle();

    A.Paragraph paragraph29 = new A.Paragraph();

    A.Field field8 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties25 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties25.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties20 = new A.ParagraphProperties();
    A.Text text25 = new A.Text();
    text25.Text = "‹#›";

    field8.Append(runProperties25);
    field8.Append(paragraphProperties20);
    field8.Append(text25);
    A.EndParagraphRunProperties endParagraphRunProperties19 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph29.Append(field8);
    paragraph29.Append(endParagraphRunProperties19);

    textBody21.Append(bodyProperties21);
    textBody21.Append(listStyle21);
    textBody21.Append(paragraph29);

    shape21.Append(nonVisualShapeProperties21);
    shape21.Append(shapeProperties22);
    shape21.Append(textBody21);

    shapeTree5.Append(nonVisualGroupShapeProperties5);
    shapeTree5.Append(groupShapeProperties5);
    shapeTree5.Append(shape17);
    shapeTree5.Append(shape18);
    shapeTree5.Append(shape19);
    shapeTree5.Append(shape20);
    shapeTree5.Append(shape21);

    commonSlideData5.Append(shapeTree5);

    ColorMapOverride colorMapOverride4 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping4 = new A.MasterColorMapping();

    colorMapOverride4.Append(masterColorMapping4);

    slideLayout3.Append(commonSlideData5);
    slideLayout3.Append(colorMapOverride4);

    slideLayoutPart3.SlideLayout = slideLayout3;
  }

  // Generates content of slideLayoutPart4.
  private void GenerateSlideLayoutPart4Content(SlideLayoutPart slideLayoutPart4) {
    SlideLayout slideLayout4 = new SlideLayout() {
      Type = SlideLayoutValues.Blank, Preserve = true
    };
    slideLayout4.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout4.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout4.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData6 = new CommonSlideData() {
      Name = "Blank"
    };

    ShapeTree shapeTree6 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties6 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties28 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties6 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties28 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties6.Append(nonVisualDrawingProperties28);
    nonVisualGroupShapeProperties6.Append(nonVisualGroupShapeDrawingProperties6);
    nonVisualGroupShapeProperties6.Append(applicationNonVisualDrawingProperties28);

    GroupShapeProperties groupShapeProperties6 = new GroupShapeProperties();

    A.TransformGroup transformGroup6 = new A.TransformGroup();
    A.Offset offset18 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents18 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset6 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents6 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup6.Append(offset18);
    transformGroup6.Append(extents18);
    transformGroup6.Append(childOffset6);
    transformGroup6.Append(childExtents6);

    groupShapeProperties6.Append(transformGroup6);

    Shape shape22 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties22 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties29 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Date Placeholder 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties22 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks22 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties22.Append(shapeLocks22);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties29 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape22 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties29.Append(placeholderShape22);

    nonVisualShapeProperties22.Append(nonVisualDrawingProperties29);
    nonVisualShapeProperties22.Append(nonVisualShapeDrawingProperties22);
    nonVisualShapeProperties22.Append(applicationNonVisualDrawingProperties29);
    ShapeProperties shapeProperties23 = new ShapeProperties();

    TextBody textBody22 = new TextBody();
    A.BodyProperties bodyProperties22 = new A.BodyProperties();
    A.ListStyle listStyle22 = new A.ListStyle();

    A.Paragraph paragraph30 = new A.Paragraph();

    A.Field field9 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties26 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties26.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties21 = new A.ParagraphProperties();
    A.Text text26 = new A.Text();
    text26.Text = "20.11.2013";

    field9.Append(runProperties26);
    field9.Append(paragraphProperties21);
    field9.Append(text26);
    A.EndParagraphRunProperties endParagraphRunProperties20 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph30.Append(field9);
    paragraph30.Append(endParagraphRunProperties20);

    textBody22.Append(bodyProperties22);
    textBody22.Append(listStyle22);
    textBody22.Append(paragraph30);

    shape22.Append(nonVisualShapeProperties22);
    shape22.Append(shapeProperties23);
    shape22.Append(textBody22);

    Shape shape23 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties23 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties30 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Footer Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties23 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks23 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties23.Append(shapeLocks23);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties30 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape23 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties30.Append(placeholderShape23);

    nonVisualShapeProperties23.Append(nonVisualDrawingProperties30);
    nonVisualShapeProperties23.Append(nonVisualShapeDrawingProperties23);
    nonVisualShapeProperties23.Append(applicationNonVisualDrawingProperties30);
    ShapeProperties shapeProperties24 = new ShapeProperties();

    TextBody textBody23 = new TextBody();
    A.BodyProperties bodyProperties23 = new A.BodyProperties();
    A.ListStyle listStyle23 = new A.ListStyle();

    A.Paragraph paragraph31 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties21 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph31.Append(endParagraphRunProperties21);

    textBody23.Append(bodyProperties23);
    textBody23.Append(listStyle23);
    textBody23.Append(paragraph31);

    shape23.Append(nonVisualShapeProperties23);
    shape23.Append(shapeProperties24);
    shape23.Append(textBody23);

    Shape shape24 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties24 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties31 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Slide Number Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties24 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks24 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties24.Append(shapeLocks24);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties31 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape24 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties31.Append(placeholderShape24);

    nonVisualShapeProperties24.Append(nonVisualDrawingProperties31);
    nonVisualShapeProperties24.Append(nonVisualShapeDrawingProperties24);
    nonVisualShapeProperties24.Append(applicationNonVisualDrawingProperties31);
    ShapeProperties shapeProperties25 = new ShapeProperties();

    TextBody textBody24 = new TextBody();
    A.BodyProperties bodyProperties24 = new A.BodyProperties();
    A.ListStyle listStyle24 = new A.ListStyle();

    A.Paragraph paragraph32 = new A.Paragraph();

    A.Field field10 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties27 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties27.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties22 = new A.ParagraphProperties();
    A.Text text27 = new A.Text();
    text27.Text = "‹#›";

    field10.Append(runProperties27);
    field10.Append(paragraphProperties22);
    field10.Append(text27);
    A.EndParagraphRunProperties endParagraphRunProperties22 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph32.Append(field10);
    paragraph32.Append(endParagraphRunProperties22);

    textBody24.Append(bodyProperties24);
    textBody24.Append(listStyle24);
    textBody24.Append(paragraph32);

    shape24.Append(nonVisualShapeProperties24);
    shape24.Append(shapeProperties25);
    shape24.Append(textBody24);

    shapeTree6.Append(nonVisualGroupShapeProperties6);
    shapeTree6.Append(groupShapeProperties6);
    shapeTree6.Append(shape22);
    shapeTree6.Append(shape23);
    shapeTree6.Append(shape24);

    commonSlideData6.Append(shapeTree6);

    ColorMapOverride colorMapOverride5 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping5 = new A.MasterColorMapping();

    colorMapOverride5.Append(masterColorMapping5);

    slideLayout4.Append(commonSlideData6);
    slideLayout4.Append(colorMapOverride5);

    slideLayoutPart4.SlideLayout = slideLayout4;
  }

  // Generates content of themePart1.
  private void GenerateThemePart1Content(ThemePart themePart1) {
    A.Theme theme1 = new A.Theme() {
      Name = "Office Theme"
    };
    theme1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

    A.ThemeElements themeElements1 = new A.ThemeElements();

    A.ColorScheme colorScheme1 = new A.ColorScheme() {
      Name = "Office"
    };

    A.Dark1Color dark1Color1 = new A.Dark1Color();
    A.SystemColor systemColor1 = new A.SystemColor() {
      Val = A.SystemColorValues.WindowText, LastColor = "000000"
    };

    dark1Color1.Append(systemColor1);

    A.Light1Color light1Color1 = new A.Light1Color();
    A.SystemColor systemColor2 = new A.SystemColor() {
      Val = A.SystemColorValues.Window, LastColor = "FFFFFF"
    };

    light1Color1.Append(systemColor2);

    A.Dark2Color dark2Color1 = new A.Dark2Color();
    A.RgbColorModelHex rgbColorModelHex1 = new A.RgbColorModelHex() {
      Val = "1F497D"
    };

    dark2Color1.Append(rgbColorModelHex1);

    A.Light2Color light2Color1 = new A.Light2Color();
    A.RgbColorModelHex rgbColorModelHex2 = new A.RgbColorModelHex() {
      Val = "EEECE1"
    };

    light2Color1.Append(rgbColorModelHex2);

    A.Accent1Color accent1Color1 = new A.Accent1Color();
    A.RgbColorModelHex rgbColorModelHex3 = new A.RgbColorModelHex() {
      Val = "4F81BD"
    };

    accent1Color1.Append(rgbColorModelHex3);

    A.Accent2Color accent2Color1 = new A.Accent2Color();
    A.RgbColorModelHex rgbColorModelHex4 = new A.RgbColorModelHex() {
      Val = "C0504D"
    };

    accent2Color1.Append(rgbColorModelHex4);

    A.Accent3Color accent3Color1 = new A.Accent3Color();
    A.RgbColorModelHex rgbColorModelHex5 = new A.RgbColorModelHex() {
      Val = "9BBB59"
    };

    accent3Color1.Append(rgbColorModelHex5);

    A.Accent4Color accent4Color1 = new A.Accent4Color();
    A.RgbColorModelHex rgbColorModelHex6 = new A.RgbColorModelHex() {
      Val = "8064A2"
    };

    accent4Color1.Append(rgbColorModelHex6);

    A.Accent5Color accent5Color1 = new A.Accent5Color();
    A.RgbColorModelHex rgbColorModelHex7 = new A.RgbColorModelHex() {
      Val = "4BACC6"
    };

    accent5Color1.Append(rgbColorModelHex7);

    A.Accent6Color accent6Color1 = new A.Accent6Color();
    A.RgbColorModelHex rgbColorModelHex8 = new A.RgbColorModelHex() {
      Val = "F79646"
    };

    accent6Color1.Append(rgbColorModelHex8);

    A.Hyperlink hyperlink1 = new A.Hyperlink();
    A.RgbColorModelHex rgbColorModelHex9 = new A.RgbColorModelHex() {
      Val = "0000FF"
    };

    hyperlink1.Append(rgbColorModelHex9);

    A.FollowedHyperlinkColor followedHyperlinkColor1 = new A.FollowedHyperlinkColor();
    A.RgbColorModelHex rgbColorModelHex10 = new A.RgbColorModelHex() {
      Val = "800080"
    };

    followedHyperlinkColor1.Append(rgbColorModelHex10);

    colorScheme1.Append(dark1Color1);
    colorScheme1.Append(light1Color1);
    colorScheme1.Append(dark2Color1);
    colorScheme1.Append(light2Color1);
    colorScheme1.Append(accent1Color1);
    colorScheme1.Append(accent2Color1);
    colorScheme1.Append(accent3Color1);
    colorScheme1.Append(accent4Color1);
    colorScheme1.Append(accent5Color1);
    colorScheme1.Append(accent6Color1);
    colorScheme1.Append(hyperlink1);
    colorScheme1.Append(followedHyperlinkColor1);

    A.FontScheme fontScheme1 = new A.FontScheme() {
      Name = "Office"
    };

    A.MajorFont majorFont1 = new A.MajorFont();
    A.LatinFont latinFont29 = new A.LatinFont() {
      Typeface = "Calibri"
    };
    A.EastAsianFont eastAsianFont29 = new A.EastAsianFont() {
      Typeface = ""
    };
    A.ComplexScriptFont complexScriptFont29 = new A.ComplexScriptFont() {
      Typeface = ""
    };
    A.SupplementalFont supplementalFont1 = new A.SupplementalFont() {
      Script = "Jpan", Typeface = "ＭＳ Ｐゴシック"
    };
    A.SupplementalFont supplementalFont2 = new A.SupplementalFont() {
      Script = "Hang", Typeface = "맑은 고딕"
    };
    A.SupplementalFont supplementalFont3 = new A.SupplementalFont() {
      Script = "Hans", Typeface = "宋体"
    };
    A.SupplementalFont supplementalFont4 = new A.SupplementalFont() {
      Script = "Hant", Typeface = "新細明體"
    };
    A.SupplementalFont supplementalFont5 = new A.SupplementalFont() {
      Script = "Arab", Typeface = "Times New Roman"
    };
    A.SupplementalFont supplementalFont6 = new A.SupplementalFont() {
      Script = "Hebr", Typeface = "Times New Roman"
    };
    A.SupplementalFont supplementalFont7 = new A.SupplementalFont() {
      Script = "Thai", Typeface = "Angsana New"
    };
    A.SupplementalFont supplementalFont8 = new A.SupplementalFont() {
      Script = "Ethi", Typeface = "Nyala"
    };
    A.SupplementalFont supplementalFont9 = new A.SupplementalFont() {
      Script = "Beng", Typeface = "Vrinda"
    };
    A.SupplementalFont supplementalFont10 = new A.SupplementalFont() {
      Script = "Gujr", Typeface = "Shruti"
    };
    A.SupplementalFont supplementalFont11 = new A.SupplementalFont() {
      Script = "Khmr", Typeface = "MoolBoran"
    };
    A.SupplementalFont supplementalFont12 = new A.SupplementalFont() {
      Script = "Knda", Typeface = "Tunga"
    };
    A.SupplementalFont supplementalFont13 = new A.SupplementalFont() {
      Script = "Guru", Typeface = "Raavi"
    };
    A.SupplementalFont supplementalFont14 = new A.SupplementalFont() {
      Script = "Cans", Typeface = "Euphemia"
    };
    A.SupplementalFont supplementalFont15 = new A.SupplementalFont() {
      Script = "Cher", Typeface = "Plantagenet Cherokee"
    };
    A.SupplementalFont supplementalFont16 = new A.SupplementalFont() {
      Script = "Yiii", Typeface = "Microsoft Yi Baiti"
    };
    A.SupplementalFont supplementalFont17 = new A.SupplementalFont() {
      Script = "Tibt", Typeface = "Microsoft Himalaya"
    };
    A.SupplementalFont supplementalFont18 = new A.SupplementalFont() {
      Script = "Thaa", Typeface = "MV Boli"
    };
    A.SupplementalFont supplementalFont19 = new A.SupplementalFont() {
      Script = "Deva", Typeface = "Mangal"
    };
    A.SupplementalFont supplementalFont20 = new A.SupplementalFont() {
      Script = "Telu", Typeface = "Gautami"
    };
    A.SupplementalFont supplementalFont21 = new A.SupplementalFont() {
      Script = "Taml", Typeface = "Latha"
    };
    A.SupplementalFont supplementalFont22 = new A.SupplementalFont() {
      Script = "Syrc", Typeface = "Estrangelo Edessa"
    };
    A.SupplementalFont supplementalFont23 = new A.SupplementalFont() {
      Script = "Orya", Typeface = "Kalinga"
    };
    A.SupplementalFont supplementalFont24 = new A.SupplementalFont() {
      Script = "Mlym", Typeface = "Kartika"
    };
    A.SupplementalFont supplementalFont25 = new A.SupplementalFont() {
      Script = "Laoo", Typeface = "DokChampa"
    };
    A.SupplementalFont supplementalFont26 = new A.SupplementalFont() {
      Script = "Sinh", Typeface = "Iskoola Pota"
    };
    A.SupplementalFont supplementalFont27 = new A.SupplementalFont() {
      Script = "Mong", Typeface = "Mongolian Baiti"
    };
    A.SupplementalFont supplementalFont28 = new A.SupplementalFont() {
      Script = "Viet", Typeface = "Times New Roman"
    };
    A.SupplementalFont supplementalFont29 = new A.SupplementalFont() {
      Script = "Uigh", Typeface = "Microsoft Uighur"
    };

    majorFont1.Append(latinFont29);
    majorFont1.Append(eastAsianFont29);
    majorFont1.Append(complexScriptFont29);
    majorFont1.Append(supplementalFont1);
    majorFont1.Append(supplementalFont2);
    majorFont1.Append(supplementalFont3);
    majorFont1.Append(supplementalFont4);
    majorFont1.Append(supplementalFont5);
    majorFont1.Append(supplementalFont6);
    majorFont1.Append(supplementalFont7);
    majorFont1.Append(supplementalFont8);
    majorFont1.Append(supplementalFont9);
    majorFont1.Append(supplementalFont10);
    majorFont1.Append(supplementalFont11);
    majorFont1.Append(supplementalFont12);
    majorFont1.Append(supplementalFont13);
    majorFont1.Append(supplementalFont14);
    majorFont1.Append(supplementalFont15);
    majorFont1.Append(supplementalFont16);
    majorFont1.Append(supplementalFont17);
    majorFont1.Append(supplementalFont18);
    majorFont1.Append(supplementalFont19);
    majorFont1.Append(supplementalFont20);
    majorFont1.Append(supplementalFont21);
    majorFont1.Append(supplementalFont22);
    majorFont1.Append(supplementalFont23);
    majorFont1.Append(supplementalFont24);
    majorFont1.Append(supplementalFont25);
    majorFont1.Append(supplementalFont26);
    majorFont1.Append(supplementalFont27);
    majorFont1.Append(supplementalFont28);
    majorFont1.Append(supplementalFont29);

    A.MinorFont minorFont1 = new A.MinorFont();
    A.LatinFont latinFont30 = new A.LatinFont() {
      Typeface = "Calibri"
    };
    A.EastAsianFont eastAsianFont30 = new A.EastAsianFont() {
      Typeface = ""
    };
    A.ComplexScriptFont complexScriptFont30 = new A.ComplexScriptFont() {
      Typeface = ""
    };
    A.SupplementalFont supplementalFont30 = new A.SupplementalFont() {
      Script = "Jpan", Typeface = "ＭＳ Ｐゴシック"
    };
    A.SupplementalFont supplementalFont31 = new A.SupplementalFont() {
      Script = "Hang", Typeface = "맑은 고딕"
    };
    A.SupplementalFont supplementalFont32 = new A.SupplementalFont() {
      Script = "Hans", Typeface = "宋体"
    };
    A.SupplementalFont supplementalFont33 = new A.SupplementalFont() {
      Script = "Hant", Typeface = "新細明體"
    };
    A.SupplementalFont supplementalFont34 = new A.SupplementalFont() {
      Script = "Arab", Typeface = "Arial"
    };
    A.SupplementalFont supplementalFont35 = new A.SupplementalFont() {
      Script = "Hebr", Typeface = "Arial"
    };
    A.SupplementalFont supplementalFont36 = new A.SupplementalFont() {
      Script = "Thai", Typeface = "Cordia New"
    };
    A.SupplementalFont supplementalFont37 = new A.SupplementalFont() {
      Script = "Ethi", Typeface = "Nyala"
    };
    A.SupplementalFont supplementalFont38 = new A.SupplementalFont() {
      Script = "Beng", Typeface = "Vrinda"
    };
    A.SupplementalFont supplementalFont39 = new A.SupplementalFont() {
      Script = "Gujr", Typeface = "Shruti"
    };
    A.SupplementalFont supplementalFont40 = new A.SupplementalFont() {
      Script = "Khmr", Typeface = "DaunPenh"
    };
    A.SupplementalFont supplementalFont41 = new A.SupplementalFont() {
      Script = "Knda", Typeface = "Tunga"
    };
    A.SupplementalFont supplementalFont42 = new A.SupplementalFont() {
      Script = "Guru", Typeface = "Raavi"
    };
    A.SupplementalFont supplementalFont43 = new A.SupplementalFont() {
      Script = "Cans", Typeface = "Euphemia"
    };
    A.SupplementalFont supplementalFont44 = new A.SupplementalFont() {
      Script = "Cher", Typeface = "Plantagenet Cherokee"
    };
    A.SupplementalFont supplementalFont45 = new A.SupplementalFont() {
      Script = "Yiii", Typeface = "Microsoft Yi Baiti"
    };
    A.SupplementalFont supplementalFont46 = new A.SupplementalFont() {
      Script = "Tibt", Typeface = "Microsoft Himalaya"
    };
    A.SupplementalFont supplementalFont47 = new A.SupplementalFont() {
      Script = "Thaa", Typeface = "MV Boli"
    };
    A.SupplementalFont supplementalFont48 = new A.SupplementalFont() {
      Script = "Deva", Typeface = "Mangal"
    };
    A.SupplementalFont supplementalFont49 = new A.SupplementalFont() {
      Script = "Telu", Typeface = "Gautami"
    };
    A.SupplementalFont supplementalFont50 = new A.SupplementalFont() {
      Script = "Taml", Typeface = "Latha"
    };
    A.SupplementalFont supplementalFont51 = new A.SupplementalFont() {
      Script = "Syrc", Typeface = "Estrangelo Edessa"
    };
    A.SupplementalFont supplementalFont52 = new A.SupplementalFont() {
      Script = "Orya", Typeface = "Kalinga"
    };
    A.SupplementalFont supplementalFont53 = new A.SupplementalFont() {
      Script = "Mlym", Typeface = "Kartika"
    };
    A.SupplementalFont supplementalFont54 = new A.SupplementalFont() {
      Script = "Laoo", Typeface = "DokChampa"
    };
    A.SupplementalFont supplementalFont55 = new A.SupplementalFont() {
      Script = "Sinh", Typeface = "Iskoola Pota"
    };
    A.SupplementalFont supplementalFont56 = new A.SupplementalFont() {
      Script = "Mong", Typeface = "Mongolian Baiti"
    };
    A.SupplementalFont supplementalFont57 = new A.SupplementalFont() {
      Script = "Viet", Typeface = "Arial"
    };
    A.SupplementalFont supplementalFont58 = new A.SupplementalFont() {
      Script = "Uigh", Typeface = "Microsoft Uighur"
    };

    minorFont1.Append(latinFont30);
    minorFont1.Append(eastAsianFont30);
    minorFont1.Append(complexScriptFont30);
    minorFont1.Append(supplementalFont30);
    minorFont1.Append(supplementalFont31);
    minorFont1.Append(supplementalFont32);
    minorFont1.Append(supplementalFont33);
    minorFont1.Append(supplementalFont34);
    minorFont1.Append(supplementalFont35);
    minorFont1.Append(supplementalFont36);
    minorFont1.Append(supplementalFont37);
    minorFont1.Append(supplementalFont38);
    minorFont1.Append(supplementalFont39);
    minorFont1.Append(supplementalFont40);
    minorFont1.Append(supplementalFont41);
    minorFont1.Append(supplementalFont42);
    minorFont1.Append(supplementalFont43);
    minorFont1.Append(supplementalFont44);
    minorFont1.Append(supplementalFont45);
    minorFont1.Append(supplementalFont46);
    minorFont1.Append(supplementalFont47);
    minorFont1.Append(supplementalFont48);
    minorFont1.Append(supplementalFont49);
    minorFont1.Append(supplementalFont50);
    minorFont1.Append(supplementalFont51);
    minorFont1.Append(supplementalFont52);
    minorFont1.Append(supplementalFont53);
    minorFont1.Append(supplementalFont54);
    minorFont1.Append(supplementalFont55);
    minorFont1.Append(supplementalFont56);
    minorFont1.Append(supplementalFont57);
    minorFont1.Append(supplementalFont58);

    fontScheme1.Append(majorFont1);
    fontScheme1.Append(minorFont1);

    A.FormatScheme formatScheme1 = new A.FormatScheme() {
      Name = "Office"
    };

    A.FillStyleList fillStyleList1 = new A.FillStyleList();

    A.SolidFill solidFill41 = new A.SolidFill();
    A.SchemeColor schemeColor42 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };

    solidFill41.Append(schemeColor42);

    A.GradientFill gradientFill1 = new A.GradientFill() {
      RotateWithShape = true
    };

    A.GradientStopList gradientStopList1 = new A.GradientStopList();

    A.GradientStop gradientStop1 = new A.GradientStop() {
      Position = 0
    };

    A.SchemeColor schemeColor43 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Tint tint13 = new A.Tint() {
      Val = 50000
    };
    A.SaturationModulation saturationModulation1 = new A.SaturationModulation() {
      Val = 300000
    };

    schemeColor43.Append(tint13);
    schemeColor43.Append(saturationModulation1);

    gradientStop1.Append(schemeColor43);

    A.GradientStop gradientStop2 = new A.GradientStop() {
      Position = 35000
    };

    A.SchemeColor schemeColor44 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Tint tint14 = new A.Tint() {
      Val = 37000
    };
    A.SaturationModulation saturationModulation2 = new A.SaturationModulation() {
      Val = 300000
    };

    schemeColor44.Append(tint14);
    schemeColor44.Append(saturationModulation2);

    gradientStop2.Append(schemeColor44);

    A.GradientStop gradientStop3 = new A.GradientStop() {
      Position = 100000
    };

    A.SchemeColor schemeColor45 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Tint tint15 = new A.Tint() {
      Val = 15000
    };
    A.SaturationModulation saturationModulation3 = new A.SaturationModulation() {
      Val = 350000
    };

    schemeColor45.Append(tint15);
    schemeColor45.Append(saturationModulation3);

    gradientStop3.Append(schemeColor45);

    gradientStopList1.Append(gradientStop1);
    gradientStopList1.Append(gradientStop2);
    gradientStopList1.Append(gradientStop3);
    A.LinearGradientFill linearGradientFill1 = new A.LinearGradientFill() {
      Angle = 16200000, Scaled = true
    };

    gradientFill1.Append(gradientStopList1);
    gradientFill1.Append(linearGradientFill1);

    A.GradientFill gradientFill2 = new A.GradientFill() {
      RotateWithShape = true
    };

    A.GradientStopList gradientStopList2 = new A.GradientStopList();

    A.GradientStop gradientStop4 = new A.GradientStop() {
      Position = 0
    };

    A.SchemeColor schemeColor46 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Shade shade1 = new A.Shade() {
      Val = 51000
    };
    A.SaturationModulation saturationModulation4 = new A.SaturationModulation() {
      Val = 130000
    };

    schemeColor46.Append(shade1);
    schemeColor46.Append(saturationModulation4);

    gradientStop4.Append(schemeColor46);

    A.GradientStop gradientStop5 = new A.GradientStop() {
      Position = 80000
    };

    A.SchemeColor schemeColor47 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Shade shade2 = new A.Shade() {
      Val = 93000
    };
    A.SaturationModulation saturationModulation5 = new A.SaturationModulation() {
      Val = 130000
    };

    schemeColor47.Append(shade2);
    schemeColor47.Append(saturationModulation5);

    gradientStop5.Append(schemeColor47);

    A.GradientStop gradientStop6 = new A.GradientStop() {
      Position = 100000
    };

    A.SchemeColor schemeColor48 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Shade shade3 = new A.Shade() {
      Val = 94000
    };
    A.SaturationModulation saturationModulation6 = new A.SaturationModulation() {
      Val = 135000
    };

    schemeColor48.Append(shade3);
    schemeColor48.Append(saturationModulation6);

    gradientStop6.Append(schemeColor48);

    gradientStopList2.Append(gradientStop4);
    gradientStopList2.Append(gradientStop5);
    gradientStopList2.Append(gradientStop6);
    A.LinearGradientFill linearGradientFill2 = new A.LinearGradientFill() {
      Angle = 16200000, Scaled = false
    };

    gradientFill2.Append(gradientStopList2);
    gradientFill2.Append(linearGradientFill2);

    fillStyleList1.Append(solidFill41);
    fillStyleList1.Append(gradientFill1);
    fillStyleList1.Append(gradientFill2);

    A.LineStyleList lineStyleList1 = new A.LineStyleList();

    A.Outline outline1 = new A.Outline() {
      Width = 9525, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center
    };

    A.SolidFill solidFill42 = new A.SolidFill();

    A.SchemeColor schemeColor49 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Shade shade4 = new A.Shade() {
      Val = 95000
    };
    A.SaturationModulation saturationModulation7 = new A.SaturationModulation() {
      Val = 105000
    };

    schemeColor49.Append(shade4);
    schemeColor49.Append(saturationModulation7);

    solidFill42.Append(schemeColor49);
    A.PresetDash presetDash1 = new A.PresetDash() {
      Val = A.PresetLineDashValues.Solid
    };

    outline1.Append(solidFill42);
    outline1.Append(presetDash1);

    A.Outline outline2 = new A.Outline() {
      Width = 25400, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center
    };

    A.SolidFill solidFill43 = new A.SolidFill();
    A.SchemeColor schemeColor50 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };

    solidFill43.Append(schemeColor50);
    A.PresetDash presetDash2 = new A.PresetDash() {
      Val = A.PresetLineDashValues.Solid
    };

    outline2.Append(solidFill43);
    outline2.Append(presetDash2);

    A.Outline outline3 = new A.Outline() {
      Width = 38100, CapType = A.LineCapValues.Flat, CompoundLineType = A.CompoundLineValues.Single, Alignment = A.PenAlignmentValues.Center
    };

    A.SolidFill solidFill44 = new A.SolidFill();
    A.SchemeColor schemeColor51 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };

    solidFill44.Append(schemeColor51);
    A.PresetDash presetDash3 = new A.PresetDash() {
      Val = A.PresetLineDashValues.Solid
    };

    outline3.Append(solidFill44);
    outline3.Append(presetDash3);

    lineStyleList1.Append(outline1);
    lineStyleList1.Append(outline2);
    lineStyleList1.Append(outline3);

    A.EffectStyleList effectStyleList1 = new A.EffectStyleList();

    A.EffectStyle effectStyle1 = new A.EffectStyle();

    A.EffectList effectList1 = new A.EffectList();

    A.OuterShadow outerShadow1 = new A.OuterShadow() {
      BlurRadius = 40000L, Distance = 20000L, Direction = 5400000, RotateWithShape = false
    };

    A.RgbColorModelHex rgbColorModelHex11 = new A.RgbColorModelHex() {
      Val = "000000"
    };
    A.Alpha alpha1 = new A.Alpha() {
      Val = 38000
    };

    rgbColorModelHex11.Append(alpha1);

    outerShadow1.Append(rgbColorModelHex11);

    effectList1.Append(outerShadow1);

    effectStyle1.Append(effectList1);

    A.EffectStyle effectStyle2 = new A.EffectStyle();

    A.EffectList effectList2 = new A.EffectList();

    A.OuterShadow outerShadow2 = new A.OuterShadow() {
      BlurRadius = 40000L, Distance = 23000L, Direction = 5400000, RotateWithShape = false
    };

    A.RgbColorModelHex rgbColorModelHex12 = new A.RgbColorModelHex() {
      Val = "000000"
    };
    A.Alpha alpha2 = new A.Alpha() {
      Val = 35000
    };

    rgbColorModelHex12.Append(alpha2);

    outerShadow2.Append(rgbColorModelHex12);

    effectList2.Append(outerShadow2);

    effectStyle2.Append(effectList2);

    A.EffectStyle effectStyle3 = new A.EffectStyle();

    A.EffectList effectList3 = new A.EffectList();

    A.OuterShadow outerShadow3 = new A.OuterShadow() {
      BlurRadius = 40000L, Distance = 23000L, Direction = 5400000, RotateWithShape = false
    };

    A.RgbColorModelHex rgbColorModelHex13 = new A.RgbColorModelHex() {
      Val = "000000"
    };
    A.Alpha alpha3 = new A.Alpha() {
      Val = 35000
    };

    rgbColorModelHex13.Append(alpha3);

    outerShadow3.Append(rgbColorModelHex13);

    effectList3.Append(outerShadow3);

    A.Scene3DType scene3DType1 = new A.Scene3DType();

    A.Camera camera1 = new A.Camera() {
      Preset = A.PresetCameraValues.OrthographicFront
    };
    A.Rotation rotation1 = new A.Rotation() {
      Latitude = 0, Longitude = 0, Revolution = 0
    };

    camera1.Append(rotation1);

    A.LightRig lightRig1 = new A.LightRig() {
      Rig = A.LightRigValues.ThreePoints, Direction = A.LightRigDirectionValues.Top
    };
    A.Rotation rotation2 = new A.Rotation() {
      Latitude = 0, Longitude = 0, Revolution = 1200000
    };

    lightRig1.Append(rotation2);

    scene3DType1.Append(camera1);
    scene3DType1.Append(lightRig1);

    A.Shape3DType shape3DType1 = new A.Shape3DType();
    A.BevelTop bevelTop1 = new A.BevelTop() {
      Width = 63500L, Height = 25400L
    };

    shape3DType1.Append(bevelTop1);

    effectStyle3.Append(effectList3);
    effectStyle3.Append(scene3DType1);
    effectStyle3.Append(shape3DType1);

    effectStyleList1.Append(effectStyle1);
    effectStyleList1.Append(effectStyle2);
    effectStyleList1.Append(effectStyle3);

    A.BackgroundFillStyleList backgroundFillStyleList1 = new A.BackgroundFillStyleList();

    A.SolidFill solidFill45 = new A.SolidFill();
    A.SchemeColor schemeColor52 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };

    solidFill45.Append(schemeColor52);

    A.GradientFill gradientFill3 = new A.GradientFill() {
      RotateWithShape = true
    };

    A.GradientStopList gradientStopList3 = new A.GradientStopList();

    A.GradientStop gradientStop7 = new A.GradientStop() {
      Position = 0
    };

    A.SchemeColor schemeColor53 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Tint tint16 = new A.Tint() {
      Val = 40000
    };
    A.SaturationModulation saturationModulation8 = new A.SaturationModulation() {
      Val = 350000
    };

    schemeColor53.Append(tint16);
    schemeColor53.Append(saturationModulation8);

    gradientStop7.Append(schemeColor53);

    A.GradientStop gradientStop8 = new A.GradientStop() {
      Position = 40000
    };

    A.SchemeColor schemeColor54 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Tint tint17 = new A.Tint() {
      Val = 45000
    };
    A.Shade shade5 = new A.Shade() {
      Val = 99000
    };
    A.SaturationModulation saturationModulation9 = new A.SaturationModulation() {
      Val = 350000
    };

    schemeColor54.Append(tint17);
    schemeColor54.Append(shade5);
    schemeColor54.Append(saturationModulation9);

    gradientStop8.Append(schemeColor54);

    A.GradientStop gradientStop9 = new A.GradientStop() {
      Position = 100000
    };

    A.SchemeColor schemeColor55 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Shade shade6 = new A.Shade() {
      Val = 20000
    };
    A.SaturationModulation saturationModulation10 = new A.SaturationModulation() {
      Val = 255000
    };

    schemeColor55.Append(shade6);
    schemeColor55.Append(saturationModulation10);

    gradientStop9.Append(schemeColor55);

    gradientStopList3.Append(gradientStop7);
    gradientStopList3.Append(gradientStop8);
    gradientStopList3.Append(gradientStop9);

    A.PathGradientFill pathGradientFill1 = new A.PathGradientFill() {
      Path = A.PathShadeValues.Circle
    };
    A.FillToRectangle fillToRectangle1 = new A.FillToRectangle() {
      Left = 50000, Top = -80000, Right = 50000, Bottom = 180000
    };

    pathGradientFill1.Append(fillToRectangle1);

    gradientFill3.Append(gradientStopList3);
    gradientFill3.Append(pathGradientFill1);

    A.GradientFill gradientFill4 = new A.GradientFill() {
      RotateWithShape = true
    };

    A.GradientStopList gradientStopList4 = new A.GradientStopList();

    A.GradientStop gradientStop10 = new A.GradientStop() {
      Position = 0
    };

    A.SchemeColor schemeColor56 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Tint tint18 = new A.Tint() {
      Val = 80000
    };
    A.SaturationModulation saturationModulation11 = new A.SaturationModulation() {
      Val = 300000
    };

    schemeColor56.Append(tint18);
    schemeColor56.Append(saturationModulation11);

    gradientStop10.Append(schemeColor56);

    A.GradientStop gradientStop11 = new A.GradientStop() {
      Position = 100000
    };

    A.SchemeColor schemeColor57 = new A.SchemeColor() {
      Val = A.SchemeColorValues.PhColor
    };
    A.Shade shade7 = new A.Shade() {
      Val = 30000
    };
    A.SaturationModulation saturationModulation12 = new A.SaturationModulation() {
      Val = 200000
    };

    schemeColor57.Append(shade7);
    schemeColor57.Append(saturationModulation12);

    gradientStop11.Append(schemeColor57);

    gradientStopList4.Append(gradientStop10);
    gradientStopList4.Append(gradientStop11);

    A.PathGradientFill pathGradientFill2 = new A.PathGradientFill() {
      Path = A.PathShadeValues.Circle
    };
    A.FillToRectangle fillToRectangle2 = new A.FillToRectangle() {
      Left = 50000, Top = 50000, Right = 50000, Bottom = 50000
    };

    pathGradientFill2.Append(fillToRectangle2);

    gradientFill4.Append(gradientStopList4);
    gradientFill4.Append(pathGradientFill2);

    backgroundFillStyleList1.Append(solidFill45);
    backgroundFillStyleList1.Append(gradientFill3);
    backgroundFillStyleList1.Append(gradientFill4);

    formatScheme1.Append(fillStyleList1);
    formatScheme1.Append(lineStyleList1);
    formatScheme1.Append(effectStyleList1);
    formatScheme1.Append(backgroundFillStyleList1);

    themeElements1.Append(colorScheme1);
    themeElements1.Append(fontScheme1);
    themeElements1.Append(formatScheme1);
    A.ObjectDefaults objectDefaults1 = new A.ObjectDefaults();
    A.ExtraColorSchemeList extraColorSchemeList1 = new A.ExtraColorSchemeList();

    theme1.Append(themeElements1);
    theme1.Append(objectDefaults1);
    theme1.Append(extraColorSchemeList1);

    themePart1.Theme = theme1;
  }

  // Generates content of slideLayoutPart5.
  private void GenerateSlideLayoutPart5Content(SlideLayoutPart slideLayoutPart5) {
    SlideLayout slideLayout5 = new SlideLayout() {
      Type = SlideLayoutValues.Object, Preserve = true
    };
    slideLayout5.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout5.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout5.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData7 = new CommonSlideData() {
      Name = "Title and Content"
    };

    ShapeTree shapeTree7 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties7 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties32 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties7 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties32 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties7.Append(nonVisualDrawingProperties32);
    nonVisualGroupShapeProperties7.Append(nonVisualGroupShapeDrawingProperties7);
    nonVisualGroupShapeProperties7.Append(applicationNonVisualDrawingProperties32);

    GroupShapeProperties groupShapeProperties7 = new GroupShapeProperties();

    A.TransformGroup transformGroup7 = new A.TransformGroup();
    A.Offset offset19 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents19 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset7 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents7 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup7.Append(offset19);
    transformGroup7.Append(extents19);
    transformGroup7.Append(childOffset7);
    transformGroup7.Append(childExtents7);

    groupShapeProperties7.Append(transformGroup7);

    Shape shape25 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties25 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties33 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties25 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks25 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties25.Append(shapeLocks25);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties33 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape25 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties33.Append(placeholderShape25);

    nonVisualShapeProperties25.Append(nonVisualDrawingProperties33);
    nonVisualShapeProperties25.Append(nonVisualShapeDrawingProperties25);
    nonVisualShapeProperties25.Append(applicationNonVisualDrawingProperties33);
    ShapeProperties shapeProperties26 = new ShapeProperties();

    TextBody textBody25 = new TextBody();
    A.BodyProperties bodyProperties25 = new A.BodyProperties();
    A.ListStyle listStyle25 = new A.ListStyle();

    A.Paragraph paragraph33 = new A.Paragraph();

    A.Run run18 = new A.Run();

    A.RunProperties runProperties28 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties28.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text28 = new A.Text();
    text28.Text = "Click to edit Master title style";

    run18.Append(runProperties28);
    run18.Append(text28);
    A.EndParagraphRunProperties endParagraphRunProperties23 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph33.Append(run18);
    paragraph33.Append(endParagraphRunProperties23);

    textBody25.Append(bodyProperties25);
    textBody25.Append(listStyle25);
    textBody25.Append(paragraph33);

    shape25.Append(nonVisualShapeProperties25);
    shape25.Append(shapeProperties26);
    shape25.Append(textBody25);

    Shape shape26 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties26 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties34 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Content Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties26 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks26 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties26.Append(shapeLocks26);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties34 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape26 = new PlaceholderShape() {
      Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties34.Append(placeholderShape26);

    nonVisualShapeProperties26.Append(nonVisualDrawingProperties34);
    nonVisualShapeProperties26.Append(nonVisualShapeDrawingProperties26);
    nonVisualShapeProperties26.Append(applicationNonVisualDrawingProperties34);
    ShapeProperties shapeProperties27 = new ShapeProperties();

    TextBody textBody26 = new TextBody();
    A.BodyProperties bodyProperties26 = new A.BodyProperties();
    A.ListStyle listStyle26 = new A.ListStyle();

    A.Paragraph paragraph34 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties23 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run19 = new A.Run();

    A.RunProperties runProperties29 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties29.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text29 = new A.Text();
    text29.Text = "Click to edit Master text styles";

    run19.Append(runProperties29);
    run19.Append(text29);

    paragraph34.Append(paragraphProperties23);
    paragraph34.Append(run19);

    A.Paragraph paragraph35 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties24 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run20 = new A.Run();

    A.RunProperties runProperties30 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties30.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text30 = new A.Text();
    text30.Text = "Second level";

    run20.Append(runProperties30);
    run20.Append(text30);

    paragraph35.Append(paragraphProperties24);
    paragraph35.Append(run20);

    A.Paragraph paragraph36 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties25 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run21 = new A.Run();

    A.RunProperties runProperties31 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties31.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text31 = new A.Text();
    text31.Text = "Third level";

    run21.Append(runProperties31);
    run21.Append(text31);

    paragraph36.Append(paragraphProperties25);
    paragraph36.Append(run21);

    A.Paragraph paragraph37 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties26 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run22 = new A.Run();

    A.RunProperties runProperties32 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties32.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text32 = new A.Text();
    text32.Text = "Fourth level";

    run22.Append(runProperties32);
    run22.Append(text32);

    paragraph37.Append(paragraphProperties26);
    paragraph37.Append(run22);

    A.Paragraph paragraph38 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties27 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run23 = new A.Run();

    A.RunProperties runProperties33 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties33.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text33 = new A.Text();
    text33.Text = "Fifth level";

    run23.Append(runProperties33);
    run23.Append(text33);
    A.EndParagraphRunProperties endParagraphRunProperties24 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph38.Append(paragraphProperties27);
    paragraph38.Append(run23);
    paragraph38.Append(endParagraphRunProperties24);

    textBody26.Append(bodyProperties26);
    textBody26.Append(listStyle26);
    textBody26.Append(paragraph34);
    textBody26.Append(paragraph35);
    textBody26.Append(paragraph36);
    textBody26.Append(paragraph37);
    textBody26.Append(paragraph38);

    shape26.Append(nonVisualShapeProperties26);
    shape26.Append(shapeProperties27);
    shape26.Append(textBody26);

    Shape shape27 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties27 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties35 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Date Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties27 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks27 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties27.Append(shapeLocks27);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties35 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape27 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties35.Append(placeholderShape27);

    nonVisualShapeProperties27.Append(nonVisualDrawingProperties35);
    nonVisualShapeProperties27.Append(nonVisualShapeDrawingProperties27);
    nonVisualShapeProperties27.Append(applicationNonVisualDrawingProperties35);
    ShapeProperties shapeProperties28 = new ShapeProperties();

    TextBody textBody27 = new TextBody();
    A.BodyProperties bodyProperties27 = new A.BodyProperties();
    A.ListStyle listStyle27 = new A.ListStyle();

    A.Paragraph paragraph39 = new A.Paragraph();

    A.Field field11 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties34 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties34.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties28 = new A.ParagraphProperties();
    A.Text text34 = new A.Text();
    text34.Text = "20.11.2013";

    field11.Append(runProperties34);
    field11.Append(paragraphProperties28);
    field11.Append(text34);
    A.EndParagraphRunProperties endParagraphRunProperties25 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph39.Append(field11);
    paragraph39.Append(endParagraphRunProperties25);

    textBody27.Append(bodyProperties27);
    textBody27.Append(listStyle27);
    textBody27.Append(paragraph39);

    shape27.Append(nonVisualShapeProperties27);
    shape27.Append(shapeProperties28);
    shape27.Append(textBody27);

    Shape shape28 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties28 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties36 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Footer Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties28 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks28 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties28.Append(shapeLocks28);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties36 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape28 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties36.Append(placeholderShape28);

    nonVisualShapeProperties28.Append(nonVisualDrawingProperties36);
    nonVisualShapeProperties28.Append(nonVisualShapeDrawingProperties28);
    nonVisualShapeProperties28.Append(applicationNonVisualDrawingProperties36);
    ShapeProperties shapeProperties29 = new ShapeProperties();

    TextBody textBody28 = new TextBody();
    A.BodyProperties bodyProperties28 = new A.BodyProperties();
    A.ListStyle listStyle28 = new A.ListStyle();

    A.Paragraph paragraph40 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties26 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph40.Append(endParagraphRunProperties26);

    textBody28.Append(bodyProperties28);
    textBody28.Append(listStyle28);
    textBody28.Append(paragraph40);

    shape28.Append(nonVisualShapeProperties28);
    shape28.Append(shapeProperties29);
    shape28.Append(textBody28);

    Shape shape29 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties29 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties37 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Slide Number Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties29 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks29 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties29.Append(shapeLocks29);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties37 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape29 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties37.Append(placeholderShape29);

    nonVisualShapeProperties29.Append(nonVisualDrawingProperties37);
    nonVisualShapeProperties29.Append(nonVisualShapeDrawingProperties29);
    nonVisualShapeProperties29.Append(applicationNonVisualDrawingProperties37);
    ShapeProperties shapeProperties30 = new ShapeProperties();

    TextBody textBody29 = new TextBody();
    A.BodyProperties bodyProperties29 = new A.BodyProperties();
    A.ListStyle listStyle29 = new A.ListStyle();

    A.Paragraph paragraph41 = new A.Paragraph();

    A.Field field12 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties35 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties35.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties29 = new A.ParagraphProperties();
    A.Text text35 = new A.Text();
    text35.Text = "‹#›";

    field12.Append(runProperties35);
    field12.Append(paragraphProperties29);
    field12.Append(text35);
    A.EndParagraphRunProperties endParagraphRunProperties27 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph41.Append(field12);
    paragraph41.Append(endParagraphRunProperties27);

    textBody29.Append(bodyProperties29);
    textBody29.Append(listStyle29);
    textBody29.Append(paragraph41);

    shape29.Append(nonVisualShapeProperties29);
    shape29.Append(shapeProperties30);
    shape29.Append(textBody29);

    shapeTree7.Append(nonVisualGroupShapeProperties7);
    shapeTree7.Append(groupShapeProperties7);
    shapeTree7.Append(shape25);
    shapeTree7.Append(shape26);
    shapeTree7.Append(shape27);
    shapeTree7.Append(shape28);
    shapeTree7.Append(shape29);

    commonSlideData7.Append(shapeTree7);

    ColorMapOverride colorMapOverride6 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping6 = new A.MasterColorMapping();

    colorMapOverride6.Append(masterColorMapping6);

    slideLayout5.Append(commonSlideData7);
    slideLayout5.Append(colorMapOverride6);

    slideLayoutPart5.SlideLayout = slideLayout5;
  }

  // Generates content of slideLayoutPart6.
  private void GenerateSlideLayoutPart6Content(SlideLayoutPart slideLayoutPart6) {
    SlideLayout slideLayout6 = new SlideLayout() {
      Type = SlideLayoutValues.Title, Preserve = true
    };
    slideLayout6.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout6.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout6.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData8 = new CommonSlideData() {
      Name = "Title Slide"
    };

    ShapeTree shapeTree8 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties8 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties38 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties8 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties38 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties8.Append(nonVisualDrawingProperties38);
    nonVisualGroupShapeProperties8.Append(nonVisualGroupShapeDrawingProperties8);
    nonVisualGroupShapeProperties8.Append(applicationNonVisualDrawingProperties38);

    GroupShapeProperties groupShapeProperties8 = new GroupShapeProperties();

    A.TransformGroup transformGroup8 = new A.TransformGroup();
    A.Offset offset20 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents20 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset8 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents8 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup8.Append(offset20);
    transformGroup8.Append(extents20);
    transformGroup8.Append(childOffset8);
    transformGroup8.Append(childExtents8);

    groupShapeProperties8.Append(transformGroup8);

    Shape shape30 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties30 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties39 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties30 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks30 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties30.Append(shapeLocks30);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties39 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape30 = new PlaceholderShape() {
      Type = PlaceholderValues.CenteredTitle
    };

    applicationNonVisualDrawingProperties39.Append(placeholderShape30);

    nonVisualShapeProperties30.Append(nonVisualDrawingProperties39);
    nonVisualShapeProperties30.Append(nonVisualShapeDrawingProperties30);
    nonVisualShapeProperties30.Append(applicationNonVisualDrawingProperties39);

    ShapeProperties shapeProperties31 = new ShapeProperties();

    A.Transform2D transform2D13 = new A.Transform2D();
    A.Offset offset21 = new A.Offset() {
      X = 685800L, Y = 2130425L
    };
    A.Extents extents21 = new A.Extents() {
      Cx = 7772400L, Cy = 1470025L
    };

    transform2D13.Append(offset21);
    transform2D13.Append(extents21);

    shapeProperties31.Append(transform2D13);

    TextBody textBody30 = new TextBody();
    A.BodyProperties bodyProperties30 = new A.BodyProperties();
    A.ListStyle listStyle30 = new A.ListStyle();

    A.Paragraph paragraph42 = new A.Paragraph();

    A.Run run24 = new A.Run();

    A.RunProperties runProperties36 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties36.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text36 = new A.Text();
    text36.Text = "Click to edit Master title style";

    run24.Append(runProperties36);
    run24.Append(text36);
    A.EndParagraphRunProperties endParagraphRunProperties28 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph42.Append(run24);
    paragraph42.Append(endParagraphRunProperties28);

    textBody30.Append(bodyProperties30);
    textBody30.Append(listStyle30);
    textBody30.Append(paragraph42);

    shape30.Append(nonVisualShapeProperties30);
    shape30.Append(shapeProperties31);
    shape30.Append(textBody30);

    Shape shape31 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties31 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties40 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Subtitle 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties31 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks31 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties31.Append(shapeLocks31);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties40 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape31 = new PlaceholderShape() {
      Type = PlaceholderValues.SubTitle, Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties40.Append(placeholderShape31);

    nonVisualShapeProperties31.Append(nonVisualDrawingProperties40);
    nonVisualShapeProperties31.Append(nonVisualShapeDrawingProperties31);
    nonVisualShapeProperties31.Append(applicationNonVisualDrawingProperties40);

    ShapeProperties shapeProperties32 = new ShapeProperties();

    A.Transform2D transform2D14 = new A.Transform2D();
    A.Offset offset22 = new A.Offset() {
      X = 1371600L, Y = 3886200L
    };
    A.Extents extents22 = new A.Extents() {
      Cx = 6400800L, Cy = 1752600L
    };

    transform2D14.Append(offset22);
    transform2D14.Append(extents22);

    shapeProperties32.Append(transform2D14);

    TextBody textBody31 = new TextBody();
    A.BodyProperties bodyProperties31 = new A.BodyProperties();

    A.ListStyle listStyle31 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties13 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet20 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties63 = new A.DefaultRunProperties();

    A.SolidFill solidFill46 = new A.SolidFill();

    A.SchemeColor schemeColor58 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint19 = new A.Tint() {
      Val = 75000
    };

    schemeColor58.Append(tint19);

    solidFill46.Append(schemeColor58);

    defaultRunProperties63.Append(solidFill46);

    level1ParagraphProperties13.Append(noBullet20);
    level1ParagraphProperties13.Append(defaultRunProperties63);

    A.Level2ParagraphProperties level2ParagraphProperties7 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet21 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties64 = new A.DefaultRunProperties();

    A.SolidFill solidFill47 = new A.SolidFill();

    A.SchemeColor schemeColor59 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint20 = new A.Tint() {
      Val = 75000
    };

    schemeColor59.Append(tint20);

    solidFill47.Append(schemeColor59);

    defaultRunProperties64.Append(solidFill47);

    level2ParagraphProperties7.Append(noBullet21);
    level2ParagraphProperties7.Append(defaultRunProperties64);

    A.Level3ParagraphProperties level3ParagraphProperties7 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet22 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties65 = new A.DefaultRunProperties();

    A.SolidFill solidFill48 = new A.SolidFill();

    A.SchemeColor schemeColor60 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint21 = new A.Tint() {
      Val = 75000
    };

    schemeColor60.Append(tint21);

    solidFill48.Append(schemeColor60);

    defaultRunProperties65.Append(solidFill48);

    level3ParagraphProperties7.Append(noBullet22);
    level3ParagraphProperties7.Append(defaultRunProperties65);

    A.Level4ParagraphProperties level4ParagraphProperties7 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet23 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties66 = new A.DefaultRunProperties();

    A.SolidFill solidFill49 = new A.SolidFill();

    A.SchemeColor schemeColor61 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint22 = new A.Tint() {
      Val = 75000
    };

    schemeColor61.Append(tint22);

    solidFill49.Append(schemeColor61);

    defaultRunProperties66.Append(solidFill49);

    level4ParagraphProperties7.Append(noBullet23);
    level4ParagraphProperties7.Append(defaultRunProperties66);

    A.Level5ParagraphProperties level5ParagraphProperties7 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet24 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties67 = new A.DefaultRunProperties();

    A.SolidFill solidFill50 = new A.SolidFill();

    A.SchemeColor schemeColor62 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint23 = new A.Tint() {
      Val = 75000
    };

    schemeColor62.Append(tint23);

    solidFill50.Append(schemeColor62);

    defaultRunProperties67.Append(solidFill50);

    level5ParagraphProperties7.Append(noBullet24);
    level5ParagraphProperties7.Append(defaultRunProperties67);

    A.Level6ParagraphProperties level6ParagraphProperties7 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet25 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties68 = new A.DefaultRunProperties();

    A.SolidFill solidFill51 = new A.SolidFill();

    A.SchemeColor schemeColor63 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint24 = new A.Tint() {
      Val = 75000
    };

    schemeColor63.Append(tint24);

    solidFill51.Append(schemeColor63);

    defaultRunProperties68.Append(solidFill51);

    level6ParagraphProperties7.Append(noBullet25);
    level6ParagraphProperties7.Append(defaultRunProperties68);

    A.Level7ParagraphProperties level7ParagraphProperties7 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet26 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties69 = new A.DefaultRunProperties();

    A.SolidFill solidFill52 = new A.SolidFill();

    A.SchemeColor schemeColor64 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint25 = new A.Tint() {
      Val = 75000
    };

    schemeColor64.Append(tint25);

    solidFill52.Append(schemeColor64);

    defaultRunProperties69.Append(solidFill52);

    level7ParagraphProperties7.Append(noBullet26);
    level7ParagraphProperties7.Append(defaultRunProperties69);

    A.Level8ParagraphProperties level8ParagraphProperties7 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet27 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties70 = new A.DefaultRunProperties();

    A.SolidFill solidFill53 = new A.SolidFill();

    A.SchemeColor schemeColor65 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint26 = new A.Tint() {
      Val = 75000
    };

    schemeColor65.Append(tint26);

    solidFill53.Append(schemeColor65);

    defaultRunProperties70.Append(solidFill53);

    level8ParagraphProperties7.Append(noBullet27);
    level8ParagraphProperties7.Append(defaultRunProperties70);

    A.Level9ParagraphProperties level9ParagraphProperties7 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Indent = 0, Alignment = A.TextAlignmentTypeValues.Center
    };
    A.NoBullet noBullet28 = new A.NoBullet();

    A.DefaultRunProperties defaultRunProperties71 = new A.DefaultRunProperties();

    A.SolidFill solidFill54 = new A.SolidFill();

    A.SchemeColor schemeColor66 = new A.SchemeColor() {
      Val = A.SchemeColorValues.Text1
    };
    A.Tint tint27 = new A.Tint() {
      Val = 75000
    };

    schemeColor66.Append(tint27);

    solidFill54.Append(schemeColor66);

    defaultRunProperties71.Append(solidFill54);

    level9ParagraphProperties7.Append(noBullet28);
    level9ParagraphProperties7.Append(defaultRunProperties71);

    listStyle31.Append(level1ParagraphProperties13);
    listStyle31.Append(level2ParagraphProperties7);
    listStyle31.Append(level3ParagraphProperties7);
    listStyle31.Append(level4ParagraphProperties7);
    listStyle31.Append(level5ParagraphProperties7);
    listStyle31.Append(level6ParagraphProperties7);
    listStyle31.Append(level7ParagraphProperties7);
    listStyle31.Append(level8ParagraphProperties7);
    listStyle31.Append(level9ParagraphProperties7);

    A.Paragraph paragraph43 = new A.Paragraph();

    A.Run run25 = new A.Run();

    A.RunProperties runProperties37 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties37.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text37 = new A.Text();
    text37.Text = "Click to edit Master subtitle style";

    run25.Append(runProperties37);
    run25.Append(text37);
    A.EndParagraphRunProperties endParagraphRunProperties29 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph43.Append(run25);
    paragraph43.Append(endParagraphRunProperties29);

    textBody31.Append(bodyProperties31);
    textBody31.Append(listStyle31);
    textBody31.Append(paragraph43);

    shape31.Append(nonVisualShapeProperties31);
    shape31.Append(shapeProperties32);
    shape31.Append(textBody31);

    Shape shape32 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties32 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties41 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Date Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties32 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks32 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties32.Append(shapeLocks32);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties41 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape32 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties41.Append(placeholderShape32);

    nonVisualShapeProperties32.Append(nonVisualDrawingProperties41);
    nonVisualShapeProperties32.Append(nonVisualShapeDrawingProperties32);
    nonVisualShapeProperties32.Append(applicationNonVisualDrawingProperties41);
    ShapeProperties shapeProperties33 = new ShapeProperties();

    TextBody textBody32 = new TextBody();
    A.BodyProperties bodyProperties32 = new A.BodyProperties();
    A.ListStyle listStyle32 = new A.ListStyle();

    A.Paragraph paragraph44 = new A.Paragraph();

    A.Field field13 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties38 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties38.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties30 = new A.ParagraphProperties();
    A.Text text38 = new A.Text();
    text38.Text = "20.11.2013";

    field13.Append(runProperties38);
    field13.Append(paragraphProperties30);
    field13.Append(text38);
    A.EndParagraphRunProperties endParagraphRunProperties30 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph44.Append(field13);
    paragraph44.Append(endParagraphRunProperties30);

    textBody32.Append(bodyProperties32);
    textBody32.Append(listStyle32);
    textBody32.Append(paragraph44);

    shape32.Append(nonVisualShapeProperties32);
    shape32.Append(shapeProperties33);
    shape32.Append(textBody32);

    Shape shape33 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties33 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties42 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Footer Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties33 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks33 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties33.Append(shapeLocks33);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties42 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape33 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties42.Append(placeholderShape33);

    nonVisualShapeProperties33.Append(nonVisualDrawingProperties42);
    nonVisualShapeProperties33.Append(nonVisualShapeDrawingProperties33);
    nonVisualShapeProperties33.Append(applicationNonVisualDrawingProperties42);
    ShapeProperties shapeProperties34 = new ShapeProperties();

    TextBody textBody33 = new TextBody();
    A.BodyProperties bodyProperties33 = new A.BodyProperties();
    A.ListStyle listStyle33 = new A.ListStyle();

    A.Paragraph paragraph45 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties31 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph45.Append(endParagraphRunProperties31);

    textBody33.Append(bodyProperties33);
    textBody33.Append(listStyle33);
    textBody33.Append(paragraph45);

    shape33.Append(nonVisualShapeProperties33);
    shape33.Append(shapeProperties34);
    shape33.Append(textBody33);

    Shape shape34 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties34 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties43 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Slide Number Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties34 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks34 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties34.Append(shapeLocks34);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties43 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape34 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties43.Append(placeholderShape34);

    nonVisualShapeProperties34.Append(nonVisualDrawingProperties43);
    nonVisualShapeProperties34.Append(nonVisualShapeDrawingProperties34);
    nonVisualShapeProperties34.Append(applicationNonVisualDrawingProperties43);
    ShapeProperties shapeProperties35 = new ShapeProperties();

    TextBody textBody34 = new TextBody();
    A.BodyProperties bodyProperties34 = new A.BodyProperties();
    A.ListStyle listStyle34 = new A.ListStyle();

    A.Paragraph paragraph46 = new A.Paragraph();

    A.Field field14 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties39 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties39.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties31 = new A.ParagraphProperties();
    A.Text text39 = new A.Text();
    text39.Text = "‹#›";

    field14.Append(runProperties39);
    field14.Append(paragraphProperties31);
    field14.Append(text39);
    A.EndParagraphRunProperties endParagraphRunProperties32 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph46.Append(field14);
    paragraph46.Append(endParagraphRunProperties32);

    textBody34.Append(bodyProperties34);
    textBody34.Append(listStyle34);
    textBody34.Append(paragraph46);

    shape34.Append(nonVisualShapeProperties34);
    shape34.Append(shapeProperties35);
    shape34.Append(textBody34);

    shapeTree8.Append(nonVisualGroupShapeProperties8);
    shapeTree8.Append(groupShapeProperties8);
    shapeTree8.Append(shape30);
    shapeTree8.Append(shape31);
    shapeTree8.Append(shape32);
    shapeTree8.Append(shape33);
    shapeTree8.Append(shape34);

    commonSlideData8.Append(shapeTree8);

    ColorMapOverride colorMapOverride7 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping7 = new A.MasterColorMapping();

    colorMapOverride7.Append(masterColorMapping7);

    slideLayout6.Append(commonSlideData8);
    slideLayout6.Append(colorMapOverride7);

    slideLayoutPart6.SlideLayout = slideLayout6;
  }

  // Generates content of slideLayoutPart7.
  private void GenerateSlideLayoutPart7Content(SlideLayoutPart slideLayoutPart7) {
    SlideLayout slideLayout7 = new SlideLayout() {
      Type = SlideLayoutValues.VerticalTitleAndText, Preserve = true
    };
    slideLayout7.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout7.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout7.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData9 = new CommonSlideData() {
      Name = "Vertical Title and Text"
    };

    ShapeTree shapeTree9 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties9 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties44 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties9 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties44 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties9.Append(nonVisualDrawingProperties44);
    nonVisualGroupShapeProperties9.Append(nonVisualGroupShapeDrawingProperties9);
    nonVisualGroupShapeProperties9.Append(applicationNonVisualDrawingProperties44);

    GroupShapeProperties groupShapeProperties9 = new GroupShapeProperties();

    A.TransformGroup transformGroup9 = new A.TransformGroup();
    A.Offset offset23 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents23 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset9 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents9 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup9.Append(offset23);
    transformGroup9.Append(extents23);
    transformGroup9.Append(childOffset9);
    transformGroup9.Append(childExtents9);

    groupShapeProperties9.Append(transformGroup9);

    Shape shape35 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties35 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties45 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Vertical Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties35 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks35 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties35.Append(shapeLocks35);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties45 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape35 = new PlaceholderShape() {
      Type = PlaceholderValues.Title, Orientation = DirectionValues.Vertical
    };

    applicationNonVisualDrawingProperties45.Append(placeholderShape35);

    nonVisualShapeProperties35.Append(nonVisualDrawingProperties45);
    nonVisualShapeProperties35.Append(nonVisualShapeDrawingProperties35);
    nonVisualShapeProperties35.Append(applicationNonVisualDrawingProperties45);

    ShapeProperties shapeProperties36 = new ShapeProperties();

    A.Transform2D transform2D15 = new A.Transform2D();
    A.Offset offset24 = new A.Offset() {
      X = 6629400L, Y = 274638L
    };
    A.Extents extents24 = new A.Extents() {
      Cx = 2057400L, Cy = 5851525L
    };

    transform2D15.Append(offset24);
    transform2D15.Append(extents24);

    shapeProperties36.Append(transform2D15);

    TextBody textBody35 = new TextBody();
    A.BodyProperties bodyProperties35 = new A.BodyProperties() {
      Vertical = A.TextVerticalValues.EastAsianVetical
    };
    A.ListStyle listStyle35 = new A.ListStyle();

    A.Paragraph paragraph47 = new A.Paragraph();

    A.Run run26 = new A.Run();

    A.RunProperties runProperties40 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties40.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text40 = new A.Text();
    text40.Text = "Click to edit Master title style";

    run26.Append(runProperties40);
    run26.Append(text40);
    A.EndParagraphRunProperties endParagraphRunProperties33 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph47.Append(run26);
    paragraph47.Append(endParagraphRunProperties33);

    textBody35.Append(bodyProperties35);
    textBody35.Append(listStyle35);
    textBody35.Append(paragraph47);

    shape35.Append(nonVisualShapeProperties35);
    shape35.Append(shapeProperties36);
    shape35.Append(textBody35);

    Shape shape36 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties36 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties46 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Vertical Text Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties36 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks36 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties36.Append(shapeLocks36);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties46 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape36 = new PlaceholderShape() {
      Type = PlaceholderValues.Body, Orientation = DirectionValues.Vertical, Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties46.Append(placeholderShape36);

    nonVisualShapeProperties36.Append(nonVisualDrawingProperties46);
    nonVisualShapeProperties36.Append(nonVisualShapeDrawingProperties36);
    nonVisualShapeProperties36.Append(applicationNonVisualDrawingProperties46);

    ShapeProperties shapeProperties37 = new ShapeProperties();

    A.Transform2D transform2D16 = new A.Transform2D();
    A.Offset offset25 = new A.Offset() {
      X = 457200L, Y = 274638L
    };
    A.Extents extents25 = new A.Extents() {
      Cx = 6019800L, Cy = 5851525L
    };

    transform2D16.Append(offset25);
    transform2D16.Append(extents25);

    shapeProperties37.Append(transform2D16);

    TextBody textBody36 = new TextBody();
    A.BodyProperties bodyProperties36 = new A.BodyProperties() {
      Vertical = A.TextVerticalValues.EastAsianVetical
    };
    A.ListStyle listStyle36 = new A.ListStyle();

    A.Paragraph paragraph48 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties32 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run27 = new A.Run();

    A.RunProperties runProperties41 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties41.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text41 = new A.Text();
    text41.Text = "Click to edit Master text styles";

    run27.Append(runProperties41);
    run27.Append(text41);

    paragraph48.Append(paragraphProperties32);
    paragraph48.Append(run27);

    A.Paragraph paragraph49 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties33 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run28 = new A.Run();

    A.RunProperties runProperties42 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties42.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text42 = new A.Text();
    text42.Text = "Second level";

    run28.Append(runProperties42);
    run28.Append(text42);

    paragraph49.Append(paragraphProperties33);
    paragraph49.Append(run28);

    A.Paragraph paragraph50 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties34 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run29 = new A.Run();

    A.RunProperties runProperties43 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties43.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text43 = new A.Text();
    text43.Text = "Third level";

    run29.Append(runProperties43);
    run29.Append(text43);

    paragraph50.Append(paragraphProperties34);
    paragraph50.Append(run29);

    A.Paragraph paragraph51 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties35 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run30 = new A.Run();

    A.RunProperties runProperties44 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties44.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text44 = new A.Text();
    text44.Text = "Fourth level";

    run30.Append(runProperties44);
    run30.Append(text44);

    paragraph51.Append(paragraphProperties35);
    paragraph51.Append(run30);

    A.Paragraph paragraph52 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties36 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run31 = new A.Run();

    A.RunProperties runProperties45 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties45.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text45 = new A.Text();
    text45.Text = "Fifth level";

    run31.Append(runProperties45);
    run31.Append(text45);
    A.EndParagraphRunProperties endParagraphRunProperties34 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph52.Append(paragraphProperties36);
    paragraph52.Append(run31);
    paragraph52.Append(endParagraphRunProperties34);

    textBody36.Append(bodyProperties36);
    textBody36.Append(listStyle36);
    textBody36.Append(paragraph48);
    textBody36.Append(paragraph49);
    textBody36.Append(paragraph50);
    textBody36.Append(paragraph51);
    textBody36.Append(paragraph52);

    shape36.Append(nonVisualShapeProperties36);
    shape36.Append(shapeProperties37);
    shape36.Append(textBody36);

    Shape shape37 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties37 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties47 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Date Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties37 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks37 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties37.Append(shapeLocks37);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties47 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape37 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties47.Append(placeholderShape37);

    nonVisualShapeProperties37.Append(nonVisualDrawingProperties47);
    nonVisualShapeProperties37.Append(nonVisualShapeDrawingProperties37);
    nonVisualShapeProperties37.Append(applicationNonVisualDrawingProperties47);
    ShapeProperties shapeProperties38 = new ShapeProperties();

    TextBody textBody37 = new TextBody();
    A.BodyProperties bodyProperties37 = new A.BodyProperties();
    A.ListStyle listStyle37 = new A.ListStyle();

    A.Paragraph paragraph53 = new A.Paragraph();

    A.Field field15 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties46 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties46.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties37 = new A.ParagraphProperties();
    A.Text text46 = new A.Text();
    text46.Text = "20.11.2013";

    field15.Append(runProperties46);
    field15.Append(paragraphProperties37);
    field15.Append(text46);
    A.EndParagraphRunProperties endParagraphRunProperties35 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph53.Append(field15);
    paragraph53.Append(endParagraphRunProperties35);

    textBody37.Append(bodyProperties37);
    textBody37.Append(listStyle37);
    textBody37.Append(paragraph53);

    shape37.Append(nonVisualShapeProperties37);
    shape37.Append(shapeProperties38);
    shape37.Append(textBody37);

    Shape shape38 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties38 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties48 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Footer Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties38 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks38 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties38.Append(shapeLocks38);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties48 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape38 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties48.Append(placeholderShape38);

    nonVisualShapeProperties38.Append(nonVisualDrawingProperties48);
    nonVisualShapeProperties38.Append(nonVisualShapeDrawingProperties38);
    nonVisualShapeProperties38.Append(applicationNonVisualDrawingProperties48);
    ShapeProperties shapeProperties39 = new ShapeProperties();

    TextBody textBody38 = new TextBody();
    A.BodyProperties bodyProperties38 = new A.BodyProperties();
    A.ListStyle listStyle38 = new A.ListStyle();

    A.Paragraph paragraph54 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties36 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph54.Append(endParagraphRunProperties36);

    textBody38.Append(bodyProperties38);
    textBody38.Append(listStyle38);
    textBody38.Append(paragraph54);

    shape38.Append(nonVisualShapeProperties38);
    shape38.Append(shapeProperties39);
    shape38.Append(textBody38);

    Shape shape39 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties39 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties49 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Slide Number Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties39 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks39 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties39.Append(shapeLocks39);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties49 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape39 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties49.Append(placeholderShape39);

    nonVisualShapeProperties39.Append(nonVisualDrawingProperties49);
    nonVisualShapeProperties39.Append(nonVisualShapeDrawingProperties39);
    nonVisualShapeProperties39.Append(applicationNonVisualDrawingProperties49);
    ShapeProperties shapeProperties40 = new ShapeProperties();

    TextBody textBody39 = new TextBody();
    A.BodyProperties bodyProperties39 = new A.BodyProperties();
    A.ListStyle listStyle39 = new A.ListStyle();

    A.Paragraph paragraph55 = new A.Paragraph();

    A.Field field16 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties47 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties47.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties38 = new A.ParagraphProperties();
    A.Text text47 = new A.Text();
    text47.Text = "‹#›";

    field16.Append(runProperties47);
    field16.Append(paragraphProperties38);
    field16.Append(text47);
    A.EndParagraphRunProperties endParagraphRunProperties37 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph55.Append(field16);
    paragraph55.Append(endParagraphRunProperties37);

    textBody39.Append(bodyProperties39);
    textBody39.Append(listStyle39);
    textBody39.Append(paragraph55);

    shape39.Append(nonVisualShapeProperties39);
    shape39.Append(shapeProperties40);
    shape39.Append(textBody39);

    shapeTree9.Append(nonVisualGroupShapeProperties9);
    shapeTree9.Append(groupShapeProperties9);
    shapeTree9.Append(shape35);
    shapeTree9.Append(shape36);
    shapeTree9.Append(shape37);
    shapeTree9.Append(shape38);
    shapeTree9.Append(shape39);

    commonSlideData9.Append(shapeTree9);

    ColorMapOverride colorMapOverride8 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping8 = new A.MasterColorMapping();

    colorMapOverride8.Append(masterColorMapping8);

    slideLayout7.Append(commonSlideData9);
    slideLayout7.Append(colorMapOverride8);

    slideLayoutPart7.SlideLayout = slideLayout7;
  }

  // Generates content of slideLayoutPart8.
  private void GenerateSlideLayoutPart8Content(SlideLayoutPart slideLayoutPart8) {
    SlideLayout slideLayout8 = new SlideLayout() {
      Type = SlideLayoutValues.TwoTextAndTwoObjects, Preserve = true
    };
    slideLayout8.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout8.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout8.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData10 = new CommonSlideData() {
      Name = "Comparison"
    };

    ShapeTree shapeTree10 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties10 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties50 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties10 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties50 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties10.Append(nonVisualDrawingProperties50);
    nonVisualGroupShapeProperties10.Append(nonVisualGroupShapeDrawingProperties10);
    nonVisualGroupShapeProperties10.Append(applicationNonVisualDrawingProperties50);

    GroupShapeProperties groupShapeProperties10 = new GroupShapeProperties();

    A.TransformGroup transformGroup10 = new A.TransformGroup();
    A.Offset offset26 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents26 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset10 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents10 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup10.Append(offset26);
    transformGroup10.Append(extents26);
    transformGroup10.Append(childOffset10);
    transformGroup10.Append(childExtents10);

    groupShapeProperties10.Append(transformGroup10);

    Shape shape40 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties40 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties51 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties40 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks40 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties40.Append(shapeLocks40);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties51 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape40 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties51.Append(placeholderShape40);

    nonVisualShapeProperties40.Append(nonVisualDrawingProperties51);
    nonVisualShapeProperties40.Append(nonVisualShapeDrawingProperties40);
    nonVisualShapeProperties40.Append(applicationNonVisualDrawingProperties51);
    ShapeProperties shapeProperties41 = new ShapeProperties();

    TextBody textBody40 = new TextBody();
    A.BodyProperties bodyProperties40 = new A.BodyProperties();

    A.ListStyle listStyle40 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties14 = new A.Level1ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties72 = new A.DefaultRunProperties();

    level1ParagraphProperties14.Append(defaultRunProperties72);

    listStyle40.Append(level1ParagraphProperties14);

    A.Paragraph paragraph56 = new A.Paragraph();

    A.Run run32 = new A.Run();

    A.RunProperties runProperties48 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties48.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text48 = new A.Text();
    text48.Text = "Click to edit Master title style";

    run32.Append(runProperties48);
    run32.Append(text48);
    A.EndParagraphRunProperties endParagraphRunProperties38 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph56.Append(run32);
    paragraph56.Append(endParagraphRunProperties38);

    textBody40.Append(bodyProperties40);
    textBody40.Append(listStyle40);
    textBody40.Append(paragraph56);

    shape40.Append(nonVisualShapeProperties40);
    shape40.Append(shapeProperties41);
    shape40.Append(textBody40);

    Shape shape41 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties41 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties52 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Text Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties41 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks41 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties41.Append(shapeLocks41);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties52 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape41 = new PlaceholderShape() {
      Type = PlaceholderValues.Body, Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties52.Append(placeholderShape41);

    nonVisualShapeProperties41.Append(nonVisualDrawingProperties52);
    nonVisualShapeProperties41.Append(nonVisualShapeDrawingProperties41);
    nonVisualShapeProperties41.Append(applicationNonVisualDrawingProperties52);

    ShapeProperties shapeProperties42 = new ShapeProperties();

    A.Transform2D transform2D17 = new A.Transform2D();
    A.Offset offset27 = new A.Offset() {
      X = 457200L, Y = 1535113L
    };
    A.Extents extents27 = new A.Extents() {
      Cx = 4040188L, Cy = 639762L
    };

    transform2D17.Append(offset27);
    transform2D17.Append(extents27);

    shapeProperties42.Append(transform2D17);

    TextBody textBody41 = new TextBody();
    A.BodyProperties bodyProperties41 = new A.BodyProperties() {
      Anchor = A.TextAnchoringTypeValues.Bottom
    };

    A.ListStyle listStyle41 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties15 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Indent = 0
    };
    A.NoBullet noBullet29 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties73 = new A.DefaultRunProperties() {
      FontSize = 2400, Bold = true
    };

    level1ParagraphProperties15.Append(noBullet29);
    level1ParagraphProperties15.Append(defaultRunProperties73);

    A.Level2ParagraphProperties level2ParagraphProperties8 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Indent = 0
    };
    A.NoBullet noBullet30 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties74 = new A.DefaultRunProperties() {
      FontSize = 2000, Bold = true
    };

    level2ParagraphProperties8.Append(noBullet30);
    level2ParagraphProperties8.Append(defaultRunProperties74);

    A.Level3ParagraphProperties level3ParagraphProperties8 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Indent = 0
    };
    A.NoBullet noBullet31 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties75 = new A.DefaultRunProperties() {
      FontSize = 1800, Bold = true
    };

    level3ParagraphProperties8.Append(noBullet31);
    level3ParagraphProperties8.Append(defaultRunProperties75);

    A.Level4ParagraphProperties level4ParagraphProperties8 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Indent = 0
    };
    A.NoBullet noBullet32 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties76 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level4ParagraphProperties8.Append(noBullet32);
    level4ParagraphProperties8.Append(defaultRunProperties76);

    A.Level5ParagraphProperties level5ParagraphProperties8 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Indent = 0
    };
    A.NoBullet noBullet33 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties77 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level5ParagraphProperties8.Append(noBullet33);
    level5ParagraphProperties8.Append(defaultRunProperties77);

    A.Level6ParagraphProperties level6ParagraphProperties8 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Indent = 0
    };
    A.NoBullet noBullet34 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties78 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level6ParagraphProperties8.Append(noBullet34);
    level6ParagraphProperties8.Append(defaultRunProperties78);

    A.Level7ParagraphProperties level7ParagraphProperties8 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Indent = 0
    };
    A.NoBullet noBullet35 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties79 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level7ParagraphProperties8.Append(noBullet35);
    level7ParagraphProperties8.Append(defaultRunProperties79);

    A.Level8ParagraphProperties level8ParagraphProperties8 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Indent = 0
    };
    A.NoBullet noBullet36 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties80 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level8ParagraphProperties8.Append(noBullet36);
    level8ParagraphProperties8.Append(defaultRunProperties80);

    A.Level9ParagraphProperties level9ParagraphProperties8 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Indent = 0
    };
    A.NoBullet noBullet37 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties81 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level9ParagraphProperties8.Append(noBullet37);
    level9ParagraphProperties8.Append(defaultRunProperties81);

    listStyle41.Append(level1ParagraphProperties15);
    listStyle41.Append(level2ParagraphProperties8);
    listStyle41.Append(level3ParagraphProperties8);
    listStyle41.Append(level4ParagraphProperties8);
    listStyle41.Append(level5ParagraphProperties8);
    listStyle41.Append(level6ParagraphProperties8);
    listStyle41.Append(level7ParagraphProperties8);
    listStyle41.Append(level8ParagraphProperties8);
    listStyle41.Append(level9ParagraphProperties8);

    A.Paragraph paragraph57 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties39 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run33 = new A.Run();

    A.RunProperties runProperties49 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties49.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text49 = new A.Text();
    text49.Text = "Click to edit Master text styles";

    run33.Append(runProperties49);
    run33.Append(text49);

    paragraph57.Append(paragraphProperties39);
    paragraph57.Append(run33);

    textBody41.Append(bodyProperties41);
    textBody41.Append(listStyle41);
    textBody41.Append(paragraph57);

    shape41.Append(nonVisualShapeProperties41);
    shape41.Append(shapeProperties42);
    shape41.Append(textBody41);

    Shape shape42 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties42 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties53 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Content Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties42 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks42 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties42.Append(shapeLocks42);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties53 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape42 = new PlaceholderShape() {
      Size = PlaceholderSizeValues.Half, Index = (UInt32Value)2U
    };

    applicationNonVisualDrawingProperties53.Append(placeholderShape42);

    nonVisualShapeProperties42.Append(nonVisualDrawingProperties53);
    nonVisualShapeProperties42.Append(nonVisualShapeDrawingProperties42);
    nonVisualShapeProperties42.Append(applicationNonVisualDrawingProperties53);

    ShapeProperties shapeProperties43 = new ShapeProperties();

    A.Transform2D transform2D18 = new A.Transform2D();
    A.Offset offset28 = new A.Offset() {
      X = 457200L, Y = 2174875L
    };
    A.Extents extents28 = new A.Extents() {
      Cx = 4040188L, Cy = 3951288L
    };

    transform2D18.Append(offset28);
    transform2D18.Append(extents28);

    shapeProperties43.Append(transform2D18);

    TextBody textBody42 = new TextBody();
    A.BodyProperties bodyProperties42 = new A.BodyProperties();

    A.ListStyle listStyle42 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties16 = new A.Level1ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties82 = new A.DefaultRunProperties() {
      FontSize = 2400
    };

    level1ParagraphProperties16.Append(defaultRunProperties82);

    A.Level2ParagraphProperties level2ParagraphProperties9 = new A.Level2ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties83 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level2ParagraphProperties9.Append(defaultRunProperties83);

    A.Level3ParagraphProperties level3ParagraphProperties9 = new A.Level3ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties84 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level3ParagraphProperties9.Append(defaultRunProperties84);

    A.Level4ParagraphProperties level4ParagraphProperties9 = new A.Level4ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties85 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level4ParagraphProperties9.Append(defaultRunProperties85);

    A.Level5ParagraphProperties level5ParagraphProperties9 = new A.Level5ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties86 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level5ParagraphProperties9.Append(defaultRunProperties86);

    A.Level6ParagraphProperties level6ParagraphProperties9 = new A.Level6ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties87 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level6ParagraphProperties9.Append(defaultRunProperties87);

    A.Level7ParagraphProperties level7ParagraphProperties9 = new A.Level7ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties88 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level7ParagraphProperties9.Append(defaultRunProperties88);

    A.Level8ParagraphProperties level8ParagraphProperties9 = new A.Level8ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties89 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level8ParagraphProperties9.Append(defaultRunProperties89);

    A.Level9ParagraphProperties level9ParagraphProperties9 = new A.Level9ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties90 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level9ParagraphProperties9.Append(defaultRunProperties90);

    listStyle42.Append(level1ParagraphProperties16);
    listStyle42.Append(level2ParagraphProperties9);
    listStyle42.Append(level3ParagraphProperties9);
    listStyle42.Append(level4ParagraphProperties9);
    listStyle42.Append(level5ParagraphProperties9);
    listStyle42.Append(level6ParagraphProperties9);
    listStyle42.Append(level7ParagraphProperties9);
    listStyle42.Append(level8ParagraphProperties9);
    listStyle42.Append(level9ParagraphProperties9);

    A.Paragraph paragraph58 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties40 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run34 = new A.Run();

    A.RunProperties runProperties50 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties50.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text50 = new A.Text();
    text50.Text = "Click to edit Master text styles";

    run34.Append(runProperties50);
    run34.Append(text50);

    paragraph58.Append(paragraphProperties40);
    paragraph58.Append(run34);

    A.Paragraph paragraph59 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties41 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run35 = new A.Run();

    A.RunProperties runProperties51 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties51.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text51 = new A.Text();
    text51.Text = "Second level";

    run35.Append(runProperties51);
    run35.Append(text51);

    paragraph59.Append(paragraphProperties41);
    paragraph59.Append(run35);

    A.Paragraph paragraph60 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties42 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run36 = new A.Run();

    A.RunProperties runProperties52 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties52.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text52 = new A.Text();
    text52.Text = "Third level";

    run36.Append(runProperties52);
    run36.Append(text52);

    paragraph60.Append(paragraphProperties42);
    paragraph60.Append(run36);

    A.Paragraph paragraph61 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties43 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run37 = new A.Run();

    A.RunProperties runProperties53 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties53.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text53 = new A.Text();
    text53.Text = "Fourth level";

    run37.Append(runProperties53);
    run37.Append(text53);

    paragraph61.Append(paragraphProperties43);
    paragraph61.Append(run37);

    A.Paragraph paragraph62 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties44 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run38 = new A.Run();

    A.RunProperties runProperties54 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties54.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text54 = new A.Text();
    text54.Text = "Fifth level";

    run38.Append(runProperties54);
    run38.Append(text54);
    A.EndParagraphRunProperties endParagraphRunProperties39 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph62.Append(paragraphProperties44);
    paragraph62.Append(run38);
    paragraph62.Append(endParagraphRunProperties39);

    textBody42.Append(bodyProperties42);
    textBody42.Append(listStyle42);
    textBody42.Append(paragraph58);
    textBody42.Append(paragraph59);
    textBody42.Append(paragraph60);
    textBody42.Append(paragraph61);
    textBody42.Append(paragraph62);

    shape42.Append(nonVisualShapeProperties42);
    shape42.Append(shapeProperties43);
    shape42.Append(textBody42);

    Shape shape43 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties43 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties54 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Text Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties43 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks43 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties43.Append(shapeLocks43);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties54 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape43 = new PlaceholderShape() {
      Type = PlaceholderValues.Body, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)3U
    };

    applicationNonVisualDrawingProperties54.Append(placeholderShape43);

    nonVisualShapeProperties43.Append(nonVisualDrawingProperties54);
    nonVisualShapeProperties43.Append(nonVisualShapeDrawingProperties43);
    nonVisualShapeProperties43.Append(applicationNonVisualDrawingProperties54);

    ShapeProperties shapeProperties44 = new ShapeProperties();

    A.Transform2D transform2D19 = new A.Transform2D();
    A.Offset offset29 = new A.Offset() {
      X = 4645025L, Y = 1535113L
    };
    A.Extents extents29 = new A.Extents() {
      Cx = 4041775L, Cy = 639762L
    };

    transform2D19.Append(offset29);
    transform2D19.Append(extents29);

    shapeProperties44.Append(transform2D19);

    TextBody textBody43 = new TextBody();
    A.BodyProperties bodyProperties43 = new A.BodyProperties() {
      Anchor = A.TextAnchoringTypeValues.Bottom
    };

    A.ListStyle listStyle43 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties17 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Indent = 0
    };
    A.NoBullet noBullet38 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties91 = new A.DefaultRunProperties() {
      FontSize = 2400, Bold = true
    };

    level1ParagraphProperties17.Append(noBullet38);
    level1ParagraphProperties17.Append(defaultRunProperties91);

    A.Level2ParagraphProperties level2ParagraphProperties10 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Indent = 0
    };
    A.NoBullet noBullet39 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties92 = new A.DefaultRunProperties() {
      FontSize = 2000, Bold = true
    };

    level2ParagraphProperties10.Append(noBullet39);
    level2ParagraphProperties10.Append(defaultRunProperties92);

    A.Level3ParagraphProperties level3ParagraphProperties10 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Indent = 0
    };
    A.NoBullet noBullet40 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties93 = new A.DefaultRunProperties() {
      FontSize = 1800, Bold = true
    };

    level3ParagraphProperties10.Append(noBullet40);
    level3ParagraphProperties10.Append(defaultRunProperties93);

    A.Level4ParagraphProperties level4ParagraphProperties10 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Indent = 0
    };
    A.NoBullet noBullet41 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties94 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level4ParagraphProperties10.Append(noBullet41);
    level4ParagraphProperties10.Append(defaultRunProperties94);

    A.Level5ParagraphProperties level5ParagraphProperties10 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Indent = 0
    };
    A.NoBullet noBullet42 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties95 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level5ParagraphProperties10.Append(noBullet42);
    level5ParagraphProperties10.Append(defaultRunProperties95);

    A.Level6ParagraphProperties level6ParagraphProperties10 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Indent = 0
    };
    A.NoBullet noBullet43 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties96 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level6ParagraphProperties10.Append(noBullet43);
    level6ParagraphProperties10.Append(defaultRunProperties96);

    A.Level7ParagraphProperties level7ParagraphProperties10 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Indent = 0
    };
    A.NoBullet noBullet44 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties97 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level7ParagraphProperties10.Append(noBullet44);
    level7ParagraphProperties10.Append(defaultRunProperties97);

    A.Level8ParagraphProperties level8ParagraphProperties10 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Indent = 0
    };
    A.NoBullet noBullet45 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties98 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level8ParagraphProperties10.Append(noBullet45);
    level8ParagraphProperties10.Append(defaultRunProperties98);

    A.Level9ParagraphProperties level9ParagraphProperties10 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Indent = 0
    };
    A.NoBullet noBullet46 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties99 = new A.DefaultRunProperties() {
      FontSize = 1600, Bold = true
    };

    level9ParagraphProperties10.Append(noBullet46);
    level9ParagraphProperties10.Append(defaultRunProperties99);

    listStyle43.Append(level1ParagraphProperties17);
    listStyle43.Append(level2ParagraphProperties10);
    listStyle43.Append(level3ParagraphProperties10);
    listStyle43.Append(level4ParagraphProperties10);
    listStyle43.Append(level5ParagraphProperties10);
    listStyle43.Append(level6ParagraphProperties10);
    listStyle43.Append(level7ParagraphProperties10);
    listStyle43.Append(level8ParagraphProperties10);
    listStyle43.Append(level9ParagraphProperties10);

    A.Paragraph paragraph63 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties45 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run39 = new A.Run();

    A.RunProperties runProperties55 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties55.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text55 = new A.Text();
    text55.Text = "Click to edit Master text styles";

    run39.Append(runProperties55);
    run39.Append(text55);

    paragraph63.Append(paragraphProperties45);
    paragraph63.Append(run39);

    textBody43.Append(bodyProperties43);
    textBody43.Append(listStyle43);
    textBody43.Append(paragraph63);

    shape43.Append(nonVisualShapeProperties43);
    shape43.Append(shapeProperties44);
    shape43.Append(textBody43);

    Shape shape44 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties44 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties55 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Content Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties44 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks44 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties44.Append(shapeLocks44);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties55 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape44 = new PlaceholderShape() {
      Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)4U
    };

    applicationNonVisualDrawingProperties55.Append(placeholderShape44);

    nonVisualShapeProperties44.Append(nonVisualDrawingProperties55);
    nonVisualShapeProperties44.Append(nonVisualShapeDrawingProperties44);
    nonVisualShapeProperties44.Append(applicationNonVisualDrawingProperties55);

    ShapeProperties shapeProperties45 = new ShapeProperties();

    A.Transform2D transform2D20 = new A.Transform2D();
    A.Offset offset30 = new A.Offset() {
      X = 4645025L, Y = 2174875L
    };
    A.Extents extents30 = new A.Extents() {
      Cx = 4041775L, Cy = 3951288L
    };

    transform2D20.Append(offset30);
    transform2D20.Append(extents30);

    shapeProperties45.Append(transform2D20);

    TextBody textBody44 = new TextBody();
    A.BodyProperties bodyProperties44 = new A.BodyProperties();

    A.ListStyle listStyle44 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties18 = new A.Level1ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties100 = new A.DefaultRunProperties() {
      FontSize = 2400
    };

    level1ParagraphProperties18.Append(defaultRunProperties100);

    A.Level2ParagraphProperties level2ParagraphProperties11 = new A.Level2ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties101 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level2ParagraphProperties11.Append(defaultRunProperties101);

    A.Level3ParagraphProperties level3ParagraphProperties11 = new A.Level3ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties102 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level3ParagraphProperties11.Append(defaultRunProperties102);

    A.Level4ParagraphProperties level4ParagraphProperties11 = new A.Level4ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties103 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level4ParagraphProperties11.Append(defaultRunProperties103);

    A.Level5ParagraphProperties level5ParagraphProperties11 = new A.Level5ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties104 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level5ParagraphProperties11.Append(defaultRunProperties104);

    A.Level6ParagraphProperties level6ParagraphProperties11 = new A.Level6ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties105 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level6ParagraphProperties11.Append(defaultRunProperties105);

    A.Level7ParagraphProperties level7ParagraphProperties11 = new A.Level7ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties106 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level7ParagraphProperties11.Append(defaultRunProperties106);

    A.Level8ParagraphProperties level8ParagraphProperties11 = new A.Level8ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties107 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level8ParagraphProperties11.Append(defaultRunProperties107);

    A.Level9ParagraphProperties level9ParagraphProperties11 = new A.Level9ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties108 = new A.DefaultRunProperties() {
      FontSize = 1600
    };

    level9ParagraphProperties11.Append(defaultRunProperties108);

    listStyle44.Append(level1ParagraphProperties18);
    listStyle44.Append(level2ParagraphProperties11);
    listStyle44.Append(level3ParagraphProperties11);
    listStyle44.Append(level4ParagraphProperties11);
    listStyle44.Append(level5ParagraphProperties11);
    listStyle44.Append(level6ParagraphProperties11);
    listStyle44.Append(level7ParagraphProperties11);
    listStyle44.Append(level8ParagraphProperties11);
    listStyle44.Append(level9ParagraphProperties11);

    A.Paragraph paragraph64 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties46 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run40 = new A.Run();

    A.RunProperties runProperties56 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties56.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text56 = new A.Text();
    text56.Text = "Click to edit Master text styles";

    run40.Append(runProperties56);
    run40.Append(text56);

    paragraph64.Append(paragraphProperties46);
    paragraph64.Append(run40);

    A.Paragraph paragraph65 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties47 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run41 = new A.Run();

    A.RunProperties runProperties57 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties57.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text57 = new A.Text();
    text57.Text = "Second level";

    run41.Append(runProperties57);
    run41.Append(text57);

    paragraph65.Append(paragraphProperties47);
    paragraph65.Append(run41);

    A.Paragraph paragraph66 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties48 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run42 = new A.Run();

    A.RunProperties runProperties58 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties58.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text58 = new A.Text();
    text58.Text = "Third level";

    run42.Append(runProperties58);
    run42.Append(text58);

    paragraph66.Append(paragraphProperties48);
    paragraph66.Append(run42);

    A.Paragraph paragraph67 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties49 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run43 = new A.Run();

    A.RunProperties runProperties59 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties59.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text59 = new A.Text();
    text59.Text = "Fourth level";

    run43.Append(runProperties59);
    run43.Append(text59);

    paragraph67.Append(paragraphProperties49);
    paragraph67.Append(run43);

    A.Paragraph paragraph68 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties50 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run44 = new A.Run();

    A.RunProperties runProperties60 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties60.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text60 = new A.Text();
    text60.Text = "Fifth level";

    run44.Append(runProperties60);
    run44.Append(text60);
    A.EndParagraphRunProperties endParagraphRunProperties40 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph68.Append(paragraphProperties50);
    paragraph68.Append(run44);
    paragraph68.Append(endParagraphRunProperties40);

    textBody44.Append(bodyProperties44);
    textBody44.Append(listStyle44);
    textBody44.Append(paragraph64);
    textBody44.Append(paragraph65);
    textBody44.Append(paragraph66);
    textBody44.Append(paragraph67);
    textBody44.Append(paragraph68);

    shape44.Append(nonVisualShapeProperties44);
    shape44.Append(shapeProperties45);
    shape44.Append(textBody44);

    Shape shape45 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties45 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties56 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)7U, Name = "Date Placeholder 6"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties45 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks45 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties45.Append(shapeLocks45);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties56 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape45 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties56.Append(placeholderShape45);

    nonVisualShapeProperties45.Append(nonVisualDrawingProperties56);
    nonVisualShapeProperties45.Append(nonVisualShapeDrawingProperties45);
    nonVisualShapeProperties45.Append(applicationNonVisualDrawingProperties56);
    ShapeProperties shapeProperties46 = new ShapeProperties();

    TextBody textBody45 = new TextBody();
    A.BodyProperties bodyProperties45 = new A.BodyProperties();
    A.ListStyle listStyle45 = new A.ListStyle();

    A.Paragraph paragraph69 = new A.Paragraph();

    A.Field field17 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties61 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties61.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties51 = new A.ParagraphProperties();
    A.Text text61 = new A.Text();
    text61.Text = "20.11.2013";

    field17.Append(runProperties61);
    field17.Append(paragraphProperties51);
    field17.Append(text61);
    A.EndParagraphRunProperties endParagraphRunProperties41 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph69.Append(field17);
    paragraph69.Append(endParagraphRunProperties41);

    textBody45.Append(bodyProperties45);
    textBody45.Append(listStyle45);
    textBody45.Append(paragraph69);

    shape45.Append(nonVisualShapeProperties45);
    shape45.Append(shapeProperties46);
    shape45.Append(textBody45);

    Shape shape46 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties46 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties57 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)8U, Name = "Footer Placeholder 7"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties46 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks46 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties46.Append(shapeLocks46);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties57 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape46 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties57.Append(placeholderShape46);

    nonVisualShapeProperties46.Append(nonVisualDrawingProperties57);
    nonVisualShapeProperties46.Append(nonVisualShapeDrawingProperties46);
    nonVisualShapeProperties46.Append(applicationNonVisualDrawingProperties57);
    ShapeProperties shapeProperties47 = new ShapeProperties();

    TextBody textBody46 = new TextBody();
    A.BodyProperties bodyProperties46 = new A.BodyProperties();
    A.ListStyle listStyle46 = new A.ListStyle();

    A.Paragraph paragraph70 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties42 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph70.Append(endParagraphRunProperties42);

    textBody46.Append(bodyProperties46);
    textBody46.Append(listStyle46);
    textBody46.Append(paragraph70);

    shape46.Append(nonVisualShapeProperties46);
    shape46.Append(shapeProperties47);
    shape46.Append(textBody46);

    Shape shape47 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties47 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties58 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)9U, Name = "Slide Number Placeholder 8"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties47 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks47 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties47.Append(shapeLocks47);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties58 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape47 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties58.Append(placeholderShape47);

    nonVisualShapeProperties47.Append(nonVisualDrawingProperties58);
    nonVisualShapeProperties47.Append(nonVisualShapeDrawingProperties47);
    nonVisualShapeProperties47.Append(applicationNonVisualDrawingProperties58);
    ShapeProperties shapeProperties48 = new ShapeProperties();

    TextBody textBody47 = new TextBody();
    A.BodyProperties bodyProperties47 = new A.BodyProperties();
    A.ListStyle listStyle47 = new A.ListStyle();

    A.Paragraph paragraph71 = new A.Paragraph();

    A.Field field18 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties62 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties62.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties52 = new A.ParagraphProperties();
    A.Text text62 = new A.Text();
    text62.Text = "‹#›";

    field18.Append(runProperties62);
    field18.Append(paragraphProperties52);
    field18.Append(text62);
    A.EndParagraphRunProperties endParagraphRunProperties43 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph71.Append(field18);
    paragraph71.Append(endParagraphRunProperties43);

    textBody47.Append(bodyProperties47);
    textBody47.Append(listStyle47);
    textBody47.Append(paragraph71);

    shape47.Append(nonVisualShapeProperties47);
    shape47.Append(shapeProperties48);
    shape47.Append(textBody47);

    shapeTree10.Append(nonVisualGroupShapeProperties10);
    shapeTree10.Append(groupShapeProperties10);
    shapeTree10.Append(shape40);
    shapeTree10.Append(shape41);
    shapeTree10.Append(shape42);
    shapeTree10.Append(shape43);
    shapeTree10.Append(shape44);
    shapeTree10.Append(shape45);
    shapeTree10.Append(shape46);
    shapeTree10.Append(shape47);

    commonSlideData10.Append(shapeTree10);

    ColorMapOverride colorMapOverride9 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping9 = new A.MasterColorMapping();

    colorMapOverride9.Append(masterColorMapping9);

    slideLayout8.Append(commonSlideData10);
    slideLayout8.Append(colorMapOverride9);

    slideLayoutPart8.SlideLayout = slideLayout8;
  }

  // Generates content of slideLayoutPart9.
  private void GenerateSlideLayoutPart9Content(SlideLayoutPart slideLayoutPart9) {
    SlideLayout slideLayout9 = new SlideLayout() {
      Type = SlideLayoutValues.VerticalText, Preserve = true
    };
    slideLayout9.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout9.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout9.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData11 = new CommonSlideData() {
      Name = "Title and Vertical Text"
    };

    ShapeTree shapeTree11 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties11 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties59 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties11 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties59 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties11.Append(nonVisualDrawingProperties59);
    nonVisualGroupShapeProperties11.Append(nonVisualGroupShapeDrawingProperties11);
    nonVisualGroupShapeProperties11.Append(applicationNonVisualDrawingProperties59);

    GroupShapeProperties groupShapeProperties11 = new GroupShapeProperties();

    A.TransformGroup transformGroup11 = new A.TransformGroup();
    A.Offset offset31 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents31 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset11 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents11 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup11.Append(offset31);
    transformGroup11.Append(extents31);
    transformGroup11.Append(childOffset11);
    transformGroup11.Append(childExtents11);

    groupShapeProperties11.Append(transformGroup11);

    Shape shape48 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties48 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties60 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties48 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks48 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties48.Append(shapeLocks48);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties60 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape48 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties60.Append(placeholderShape48);

    nonVisualShapeProperties48.Append(nonVisualDrawingProperties60);
    nonVisualShapeProperties48.Append(nonVisualShapeDrawingProperties48);
    nonVisualShapeProperties48.Append(applicationNonVisualDrawingProperties60);
    ShapeProperties shapeProperties49 = new ShapeProperties();

    TextBody textBody48 = new TextBody();
    A.BodyProperties bodyProperties48 = new A.BodyProperties();
    A.ListStyle listStyle48 = new A.ListStyle();

    A.Paragraph paragraph72 = new A.Paragraph();

    A.Run run45 = new A.Run();

    A.RunProperties runProperties63 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties63.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text63 = new A.Text();
    text63.Text = "Click to edit Master title style";

    run45.Append(runProperties63);
    run45.Append(text63);
    A.EndParagraphRunProperties endParagraphRunProperties44 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph72.Append(run45);
    paragraph72.Append(endParagraphRunProperties44);

    textBody48.Append(bodyProperties48);
    textBody48.Append(listStyle48);
    textBody48.Append(paragraph72);

    shape48.Append(nonVisualShapeProperties48);
    shape48.Append(shapeProperties49);
    shape48.Append(textBody48);

    Shape shape49 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties49 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties61 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Vertical Text Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties49 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks49 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties49.Append(shapeLocks49);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties61 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape49 = new PlaceholderShape() {
      Type = PlaceholderValues.Body, Orientation = DirectionValues.Vertical, Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties61.Append(placeholderShape49);

    nonVisualShapeProperties49.Append(nonVisualDrawingProperties61);
    nonVisualShapeProperties49.Append(nonVisualShapeDrawingProperties49);
    nonVisualShapeProperties49.Append(applicationNonVisualDrawingProperties61);
    ShapeProperties shapeProperties50 = new ShapeProperties();

    TextBody textBody49 = new TextBody();
    A.BodyProperties bodyProperties49 = new A.BodyProperties() {
      Vertical = A.TextVerticalValues.EastAsianVetical
    };
    A.ListStyle listStyle49 = new A.ListStyle();

    A.Paragraph paragraph73 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties53 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run46 = new A.Run();

    A.RunProperties runProperties64 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties64.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text64 = new A.Text();
    text64.Text = "Click to edit Master text styles";

    run46.Append(runProperties64);
    run46.Append(text64);

    paragraph73.Append(paragraphProperties53);
    paragraph73.Append(run46);

    A.Paragraph paragraph74 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties54 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run47 = new A.Run();

    A.RunProperties runProperties65 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties65.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text65 = new A.Text();
    text65.Text = "Second level";

    run47.Append(runProperties65);
    run47.Append(text65);

    paragraph74.Append(paragraphProperties54);
    paragraph74.Append(run47);

    A.Paragraph paragraph75 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties55 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run48 = new A.Run();

    A.RunProperties runProperties66 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties66.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text66 = new A.Text();
    text66.Text = "Third level";

    run48.Append(runProperties66);
    run48.Append(text66);

    paragraph75.Append(paragraphProperties55);
    paragraph75.Append(run48);

    A.Paragraph paragraph76 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties56 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run49 = new A.Run();

    A.RunProperties runProperties67 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties67.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text67 = new A.Text();
    text67.Text = "Fourth level";

    run49.Append(runProperties67);
    run49.Append(text67);

    paragraph76.Append(paragraphProperties56);
    paragraph76.Append(run49);

    A.Paragraph paragraph77 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties57 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run50 = new A.Run();

    A.RunProperties runProperties68 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties68.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text68 = new A.Text();
    text68.Text = "Fifth level";

    run50.Append(runProperties68);
    run50.Append(text68);
    A.EndParagraphRunProperties endParagraphRunProperties45 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph77.Append(paragraphProperties57);
    paragraph77.Append(run50);
    paragraph77.Append(endParagraphRunProperties45);

    textBody49.Append(bodyProperties49);
    textBody49.Append(listStyle49);
    textBody49.Append(paragraph73);
    textBody49.Append(paragraph74);
    textBody49.Append(paragraph75);
    textBody49.Append(paragraph76);
    textBody49.Append(paragraph77);

    shape49.Append(nonVisualShapeProperties49);
    shape49.Append(shapeProperties50);
    shape49.Append(textBody49);

    Shape shape50 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties50 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties62 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Date Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties50 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks50 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties50.Append(shapeLocks50);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties62 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape50 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties62.Append(placeholderShape50);

    nonVisualShapeProperties50.Append(nonVisualDrawingProperties62);
    nonVisualShapeProperties50.Append(nonVisualShapeDrawingProperties50);
    nonVisualShapeProperties50.Append(applicationNonVisualDrawingProperties62);
    ShapeProperties shapeProperties51 = new ShapeProperties();

    TextBody textBody50 = new TextBody();
    A.BodyProperties bodyProperties50 = new A.BodyProperties();
    A.ListStyle listStyle50 = new A.ListStyle();

    A.Paragraph paragraph78 = new A.Paragraph();

    A.Field field19 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties69 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties69.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties58 = new A.ParagraphProperties();
    A.Text text69 = new A.Text();
    text69.Text = "20.11.2013";

    field19.Append(runProperties69);
    field19.Append(paragraphProperties58);
    field19.Append(text69);
    A.EndParagraphRunProperties endParagraphRunProperties46 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph78.Append(field19);
    paragraph78.Append(endParagraphRunProperties46);

    textBody50.Append(bodyProperties50);
    textBody50.Append(listStyle50);
    textBody50.Append(paragraph78);

    shape50.Append(nonVisualShapeProperties50);
    shape50.Append(shapeProperties51);
    shape50.Append(textBody50);

    Shape shape51 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties51 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties63 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Footer Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties51 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks51 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties51.Append(shapeLocks51);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties63 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape51 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties63.Append(placeholderShape51);

    nonVisualShapeProperties51.Append(nonVisualDrawingProperties63);
    nonVisualShapeProperties51.Append(nonVisualShapeDrawingProperties51);
    nonVisualShapeProperties51.Append(applicationNonVisualDrawingProperties63);
    ShapeProperties shapeProperties52 = new ShapeProperties();

    TextBody textBody51 = new TextBody();
    A.BodyProperties bodyProperties51 = new A.BodyProperties();
    A.ListStyle listStyle51 = new A.ListStyle();

    A.Paragraph paragraph79 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties47 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph79.Append(endParagraphRunProperties47);

    textBody51.Append(bodyProperties51);
    textBody51.Append(listStyle51);
    textBody51.Append(paragraph79);

    shape51.Append(nonVisualShapeProperties51);
    shape51.Append(shapeProperties52);
    shape51.Append(textBody51);

    Shape shape52 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties52 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties64 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Slide Number Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties52 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks52 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties52.Append(shapeLocks52);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties64 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape52 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties64.Append(placeholderShape52);

    nonVisualShapeProperties52.Append(nonVisualDrawingProperties64);
    nonVisualShapeProperties52.Append(nonVisualShapeDrawingProperties52);
    nonVisualShapeProperties52.Append(applicationNonVisualDrawingProperties64);
    ShapeProperties shapeProperties53 = new ShapeProperties();

    TextBody textBody52 = new TextBody();
    A.BodyProperties bodyProperties52 = new A.BodyProperties();
    A.ListStyle listStyle52 = new A.ListStyle();

    A.Paragraph paragraph80 = new A.Paragraph();

    A.Field field20 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties70 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties70.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties59 = new A.ParagraphProperties();
    A.Text text70 = new A.Text();
    text70.Text = "‹#›";

    field20.Append(runProperties70);
    field20.Append(paragraphProperties59);
    field20.Append(text70);
    A.EndParagraphRunProperties endParagraphRunProperties48 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph80.Append(field20);
    paragraph80.Append(endParagraphRunProperties48);

    textBody52.Append(bodyProperties52);
    textBody52.Append(listStyle52);
    textBody52.Append(paragraph80);

    shape52.Append(nonVisualShapeProperties52);
    shape52.Append(shapeProperties53);
    shape52.Append(textBody52);

    shapeTree11.Append(nonVisualGroupShapeProperties11);
    shapeTree11.Append(groupShapeProperties11);
    shapeTree11.Append(shape48);
    shapeTree11.Append(shape49);
    shapeTree11.Append(shape50);
    shapeTree11.Append(shape51);
    shapeTree11.Append(shape52);

    commonSlideData11.Append(shapeTree11);

    ColorMapOverride colorMapOverride10 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping10 = new A.MasterColorMapping();

    colorMapOverride10.Append(masterColorMapping10);

    slideLayout9.Append(commonSlideData11);
    slideLayout9.Append(colorMapOverride10);

    slideLayoutPart9.SlideLayout = slideLayout9;
  }

  // Generates content of slideLayoutPart10.
  private void GenerateSlideLayoutPart10Content(SlideLayoutPart slideLayoutPart10) {
    SlideLayout slideLayout10 = new SlideLayout() {
      Type = SlideLayoutValues.TwoObjects, Preserve = true
    };
    slideLayout10.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout10.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout10.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData12 = new CommonSlideData() {
      Name = "Two Content"
    };

    ShapeTree shapeTree12 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties12 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties65 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties12 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties65 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties12.Append(nonVisualDrawingProperties65);
    nonVisualGroupShapeProperties12.Append(nonVisualGroupShapeDrawingProperties12);
    nonVisualGroupShapeProperties12.Append(applicationNonVisualDrawingProperties65);

    GroupShapeProperties groupShapeProperties12 = new GroupShapeProperties();

    A.TransformGroup transformGroup12 = new A.TransformGroup();
    A.Offset offset32 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents32 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset12 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents12 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup12.Append(offset32);
    transformGroup12.Append(extents32);
    transformGroup12.Append(childOffset12);
    transformGroup12.Append(childExtents12);

    groupShapeProperties12.Append(transformGroup12);

    Shape shape53 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties53 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties66 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties53 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks53 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties53.Append(shapeLocks53);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties66 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape53 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties66.Append(placeholderShape53);

    nonVisualShapeProperties53.Append(nonVisualDrawingProperties66);
    nonVisualShapeProperties53.Append(nonVisualShapeDrawingProperties53);
    nonVisualShapeProperties53.Append(applicationNonVisualDrawingProperties66);
    ShapeProperties shapeProperties54 = new ShapeProperties();

    TextBody textBody53 = new TextBody();
    A.BodyProperties bodyProperties53 = new A.BodyProperties();
    A.ListStyle listStyle53 = new A.ListStyle();

    A.Paragraph paragraph81 = new A.Paragraph();

    A.Run run51 = new A.Run();

    A.RunProperties runProperties71 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties71.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text71 = new A.Text();
    text71.Text = "Click to edit Master title style";

    run51.Append(runProperties71);
    run51.Append(text71);
    A.EndParagraphRunProperties endParagraphRunProperties49 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph81.Append(run51);
    paragraph81.Append(endParagraphRunProperties49);

    textBody53.Append(bodyProperties53);
    textBody53.Append(listStyle53);
    textBody53.Append(paragraph81);

    shape53.Append(nonVisualShapeProperties53);
    shape53.Append(shapeProperties54);
    shape53.Append(textBody53);

    Shape shape54 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties54 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties67 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Content Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties54 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks54 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties54.Append(shapeLocks54);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties67 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape54 = new PlaceholderShape() {
      Size = PlaceholderSizeValues.Half, Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties67.Append(placeholderShape54);

    nonVisualShapeProperties54.Append(nonVisualDrawingProperties67);
    nonVisualShapeProperties54.Append(nonVisualShapeDrawingProperties54);
    nonVisualShapeProperties54.Append(applicationNonVisualDrawingProperties67);

    ShapeProperties shapeProperties55 = new ShapeProperties();

    A.Transform2D transform2D21 = new A.Transform2D();
    A.Offset offset33 = new A.Offset() {
      X = 457200L, Y = 1600200L
    };
    A.Extents extents33 = new A.Extents() {
      Cx = 4038600L, Cy = 4525963L
    };

    transform2D21.Append(offset33);
    transform2D21.Append(extents33);

    shapeProperties55.Append(transform2D21);

    TextBody textBody54 = new TextBody();
    A.BodyProperties bodyProperties54 = new A.BodyProperties();

    A.ListStyle listStyle54 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties19 = new A.Level1ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties109 = new A.DefaultRunProperties() {
      FontSize = 2800
    };

    level1ParagraphProperties19.Append(defaultRunProperties109);

    A.Level2ParagraphProperties level2ParagraphProperties12 = new A.Level2ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties110 = new A.DefaultRunProperties() {
      FontSize = 2400
    };

    level2ParagraphProperties12.Append(defaultRunProperties110);

    A.Level3ParagraphProperties level3ParagraphProperties12 = new A.Level3ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties111 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level3ParagraphProperties12.Append(defaultRunProperties111);

    A.Level4ParagraphProperties level4ParagraphProperties12 = new A.Level4ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties112 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level4ParagraphProperties12.Append(defaultRunProperties112);

    A.Level5ParagraphProperties level5ParagraphProperties12 = new A.Level5ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties113 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level5ParagraphProperties12.Append(defaultRunProperties113);

    A.Level6ParagraphProperties level6ParagraphProperties12 = new A.Level6ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties114 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level6ParagraphProperties12.Append(defaultRunProperties114);

    A.Level7ParagraphProperties level7ParagraphProperties12 = new A.Level7ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties115 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level7ParagraphProperties12.Append(defaultRunProperties115);

    A.Level8ParagraphProperties level8ParagraphProperties12 = new A.Level8ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties116 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level8ParagraphProperties12.Append(defaultRunProperties116);

    A.Level9ParagraphProperties level9ParagraphProperties12 = new A.Level9ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties117 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level9ParagraphProperties12.Append(defaultRunProperties117);

    listStyle54.Append(level1ParagraphProperties19);
    listStyle54.Append(level2ParagraphProperties12);
    listStyle54.Append(level3ParagraphProperties12);
    listStyle54.Append(level4ParagraphProperties12);
    listStyle54.Append(level5ParagraphProperties12);
    listStyle54.Append(level6ParagraphProperties12);
    listStyle54.Append(level7ParagraphProperties12);
    listStyle54.Append(level8ParagraphProperties12);
    listStyle54.Append(level9ParagraphProperties12);

    A.Paragraph paragraph82 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties60 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run52 = new A.Run();

    A.RunProperties runProperties72 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties72.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text72 = new A.Text();
    text72.Text = "Click to edit Master text styles";

    run52.Append(runProperties72);
    run52.Append(text72);

    paragraph82.Append(paragraphProperties60);
    paragraph82.Append(run52);

    A.Paragraph paragraph83 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties61 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run53 = new A.Run();

    A.RunProperties runProperties73 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties73.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text73 = new A.Text();
    text73.Text = "Second level";

    run53.Append(runProperties73);
    run53.Append(text73);

    paragraph83.Append(paragraphProperties61);
    paragraph83.Append(run53);

    A.Paragraph paragraph84 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties62 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run54 = new A.Run();

    A.RunProperties runProperties74 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties74.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text74 = new A.Text();
    text74.Text = "Third level";

    run54.Append(runProperties74);
    run54.Append(text74);

    paragraph84.Append(paragraphProperties62);
    paragraph84.Append(run54);

    A.Paragraph paragraph85 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties63 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run55 = new A.Run();

    A.RunProperties runProperties75 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties75.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text75 = new A.Text();
    text75.Text = "Fourth level";

    run55.Append(runProperties75);
    run55.Append(text75);

    paragraph85.Append(paragraphProperties63);
    paragraph85.Append(run55);

    A.Paragraph paragraph86 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties64 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run56 = new A.Run();

    A.RunProperties runProperties76 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties76.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text76 = new A.Text();
    text76.Text = "Fifth level";

    run56.Append(runProperties76);
    run56.Append(text76);
    A.EndParagraphRunProperties endParagraphRunProperties50 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph86.Append(paragraphProperties64);
    paragraph86.Append(run56);
    paragraph86.Append(endParagraphRunProperties50);

    textBody54.Append(bodyProperties54);
    textBody54.Append(listStyle54);
    textBody54.Append(paragraph82);
    textBody54.Append(paragraph83);
    textBody54.Append(paragraph84);
    textBody54.Append(paragraph85);
    textBody54.Append(paragraph86);

    shape54.Append(nonVisualShapeProperties54);
    shape54.Append(shapeProperties55);
    shape54.Append(textBody54);

    Shape shape55 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties55 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties68 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Content Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties55 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks55 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties55.Append(shapeLocks55);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties68 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape55 = new PlaceholderShape() {
      Size = PlaceholderSizeValues.Half, Index = (UInt32Value)2U
    };

    applicationNonVisualDrawingProperties68.Append(placeholderShape55);

    nonVisualShapeProperties55.Append(nonVisualDrawingProperties68);
    nonVisualShapeProperties55.Append(nonVisualShapeDrawingProperties55);
    nonVisualShapeProperties55.Append(applicationNonVisualDrawingProperties68);

    ShapeProperties shapeProperties56 = new ShapeProperties();

    A.Transform2D transform2D22 = new A.Transform2D();
    A.Offset offset34 = new A.Offset() {
      X = 4648200L, Y = 1600200L
    };
    A.Extents extents34 = new A.Extents() {
      Cx = 4038600L, Cy = 4525963L
    };

    transform2D22.Append(offset34);
    transform2D22.Append(extents34);

    shapeProperties56.Append(transform2D22);

    TextBody textBody55 = new TextBody();
    A.BodyProperties bodyProperties55 = new A.BodyProperties();

    A.ListStyle listStyle55 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties20 = new A.Level1ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties118 = new A.DefaultRunProperties() {
      FontSize = 2800
    };

    level1ParagraphProperties20.Append(defaultRunProperties118);

    A.Level2ParagraphProperties level2ParagraphProperties13 = new A.Level2ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties119 = new A.DefaultRunProperties() {
      FontSize = 2400
    };

    level2ParagraphProperties13.Append(defaultRunProperties119);

    A.Level3ParagraphProperties level3ParagraphProperties13 = new A.Level3ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties120 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level3ParagraphProperties13.Append(defaultRunProperties120);

    A.Level4ParagraphProperties level4ParagraphProperties13 = new A.Level4ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties121 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level4ParagraphProperties13.Append(defaultRunProperties121);

    A.Level5ParagraphProperties level5ParagraphProperties13 = new A.Level5ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties122 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level5ParagraphProperties13.Append(defaultRunProperties122);

    A.Level6ParagraphProperties level6ParagraphProperties13 = new A.Level6ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties123 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level6ParagraphProperties13.Append(defaultRunProperties123);

    A.Level7ParagraphProperties level7ParagraphProperties13 = new A.Level7ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties124 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level7ParagraphProperties13.Append(defaultRunProperties124);

    A.Level8ParagraphProperties level8ParagraphProperties13 = new A.Level8ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties125 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level8ParagraphProperties13.Append(defaultRunProperties125);

    A.Level9ParagraphProperties level9ParagraphProperties13 = new A.Level9ParagraphProperties();
    A.DefaultRunProperties defaultRunProperties126 = new A.DefaultRunProperties() {
      FontSize = 1800
    };

    level9ParagraphProperties13.Append(defaultRunProperties126);

    listStyle55.Append(level1ParagraphProperties20);
    listStyle55.Append(level2ParagraphProperties13);
    listStyle55.Append(level3ParagraphProperties13);
    listStyle55.Append(level4ParagraphProperties13);
    listStyle55.Append(level5ParagraphProperties13);
    listStyle55.Append(level6ParagraphProperties13);
    listStyle55.Append(level7ParagraphProperties13);
    listStyle55.Append(level8ParagraphProperties13);
    listStyle55.Append(level9ParagraphProperties13);

    A.Paragraph paragraph87 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties65 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run57 = new A.Run();

    A.RunProperties runProperties77 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties77.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text77 = new A.Text();
    text77.Text = "Click to edit Master text styles";

    run57.Append(runProperties77);
    run57.Append(text77);

    paragraph87.Append(paragraphProperties65);
    paragraph87.Append(run57);

    A.Paragraph paragraph88 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties66 = new A.ParagraphProperties() {
      Level = 1
    };

    A.Run run58 = new A.Run();

    A.RunProperties runProperties78 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties78.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text78 = new A.Text();
    text78.Text = "Second level";

    run58.Append(runProperties78);
    run58.Append(text78);

    paragraph88.Append(paragraphProperties66);
    paragraph88.Append(run58);

    A.Paragraph paragraph89 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties67 = new A.ParagraphProperties() {
      Level = 2
    };

    A.Run run59 = new A.Run();

    A.RunProperties runProperties79 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties79.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text79 = new A.Text();
    text79.Text = "Third level";

    run59.Append(runProperties79);
    run59.Append(text79);

    paragraph89.Append(paragraphProperties67);
    paragraph89.Append(run59);

    A.Paragraph paragraph90 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties68 = new A.ParagraphProperties() {
      Level = 3
    };

    A.Run run60 = new A.Run();

    A.RunProperties runProperties80 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties80.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text80 = new A.Text();
    text80.Text = "Fourth level";

    run60.Append(runProperties80);
    run60.Append(text80);

    paragraph90.Append(paragraphProperties68);
    paragraph90.Append(run60);

    A.Paragraph paragraph91 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties69 = new A.ParagraphProperties() {
      Level = 4
    };

    A.Run run61 = new A.Run();

    A.RunProperties runProperties81 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties81.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text81 = new A.Text();
    text81.Text = "Fifth level";

    run61.Append(runProperties81);
    run61.Append(text81);
    A.EndParagraphRunProperties endParagraphRunProperties51 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph91.Append(paragraphProperties69);
    paragraph91.Append(run61);
    paragraph91.Append(endParagraphRunProperties51);

    textBody55.Append(bodyProperties55);
    textBody55.Append(listStyle55);
    textBody55.Append(paragraph87);
    textBody55.Append(paragraph88);
    textBody55.Append(paragraph89);
    textBody55.Append(paragraph90);
    textBody55.Append(paragraph91);

    shape55.Append(nonVisualShapeProperties55);
    shape55.Append(shapeProperties56);
    shape55.Append(textBody55);

    Shape shape56 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties56 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties69 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Date Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties56 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks56 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties56.Append(shapeLocks56);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties69 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape56 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties69.Append(placeholderShape56);

    nonVisualShapeProperties56.Append(nonVisualDrawingProperties69);
    nonVisualShapeProperties56.Append(nonVisualShapeDrawingProperties56);
    nonVisualShapeProperties56.Append(applicationNonVisualDrawingProperties69);
    ShapeProperties shapeProperties57 = new ShapeProperties();

    TextBody textBody56 = new TextBody();
    A.BodyProperties bodyProperties56 = new A.BodyProperties();
    A.ListStyle listStyle56 = new A.ListStyle();

    A.Paragraph paragraph92 = new A.Paragraph();

    A.Field field21 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties82 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties82.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties70 = new A.ParagraphProperties();
    A.Text text82 = new A.Text();
    text82.Text = "20.11.2013";

    field21.Append(runProperties82);
    field21.Append(paragraphProperties70);
    field21.Append(text82);
    A.EndParagraphRunProperties endParagraphRunProperties52 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph92.Append(field21);
    paragraph92.Append(endParagraphRunProperties52);

    textBody56.Append(bodyProperties56);
    textBody56.Append(listStyle56);
    textBody56.Append(paragraph92);

    shape56.Append(nonVisualShapeProperties56);
    shape56.Append(shapeProperties57);
    shape56.Append(textBody56);

    Shape shape57 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties57 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties70 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Footer Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties57 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks57 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties57.Append(shapeLocks57);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties70 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape57 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties70.Append(placeholderShape57);

    nonVisualShapeProperties57.Append(nonVisualDrawingProperties70);
    nonVisualShapeProperties57.Append(nonVisualShapeDrawingProperties57);
    nonVisualShapeProperties57.Append(applicationNonVisualDrawingProperties70);
    ShapeProperties shapeProperties58 = new ShapeProperties();

    TextBody textBody57 = new TextBody();
    A.BodyProperties bodyProperties57 = new A.BodyProperties();
    A.ListStyle listStyle57 = new A.ListStyle();

    A.Paragraph paragraph93 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties53 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph93.Append(endParagraphRunProperties53);

    textBody57.Append(bodyProperties57);
    textBody57.Append(listStyle57);
    textBody57.Append(paragraph93);

    shape57.Append(nonVisualShapeProperties57);
    shape57.Append(shapeProperties58);
    shape57.Append(textBody57);

    Shape shape58 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties58 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties71 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)7U, Name = "Slide Number Placeholder 6"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties58 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks58 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties58.Append(shapeLocks58);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties71 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape58 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties71.Append(placeholderShape58);

    nonVisualShapeProperties58.Append(nonVisualDrawingProperties71);
    nonVisualShapeProperties58.Append(nonVisualShapeDrawingProperties58);
    nonVisualShapeProperties58.Append(applicationNonVisualDrawingProperties71);
    ShapeProperties shapeProperties59 = new ShapeProperties();

    TextBody textBody58 = new TextBody();
    A.BodyProperties bodyProperties58 = new A.BodyProperties();
    A.ListStyle listStyle58 = new A.ListStyle();

    A.Paragraph paragraph94 = new A.Paragraph();

    A.Field field22 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties83 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties83.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties71 = new A.ParagraphProperties();
    A.Text text83 = new A.Text();
    text83.Text = "‹#›";

    field22.Append(runProperties83);
    field22.Append(paragraphProperties71);
    field22.Append(text83);
    A.EndParagraphRunProperties endParagraphRunProperties54 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph94.Append(field22);
    paragraph94.Append(endParagraphRunProperties54);

    textBody58.Append(bodyProperties58);
    textBody58.Append(listStyle58);
    textBody58.Append(paragraph94);

    shape58.Append(nonVisualShapeProperties58);
    shape58.Append(shapeProperties59);
    shape58.Append(textBody58);

    shapeTree12.Append(nonVisualGroupShapeProperties12);
    shapeTree12.Append(groupShapeProperties12);
    shapeTree12.Append(shape53);
    shapeTree12.Append(shape54);
    shapeTree12.Append(shape55);
    shapeTree12.Append(shape56);
    shapeTree12.Append(shape57);
    shapeTree12.Append(shape58);

    commonSlideData12.Append(shapeTree12);

    ColorMapOverride colorMapOverride11 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping11 = new A.MasterColorMapping();

    colorMapOverride11.Append(masterColorMapping11);

    slideLayout10.Append(commonSlideData12);
    slideLayout10.Append(colorMapOverride11);

    slideLayoutPart10.SlideLayout = slideLayout10;
  }

  // Generates content of slideLayoutPart11.
  private void GenerateSlideLayoutPart11Content(SlideLayoutPart slideLayoutPart11) {
    SlideLayout slideLayout11 = new SlideLayout() {
      Type = SlideLayoutValues.PictureText, Preserve = true
    };
    slideLayout11.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slideLayout11.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slideLayout11.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData13 = new CommonSlideData() {
      Name = "Picture with Caption"
    };

    ShapeTree shapeTree13 = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties13 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties72 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties13 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties72 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties13.Append(nonVisualDrawingProperties72);
    nonVisualGroupShapeProperties13.Append(nonVisualGroupShapeDrawingProperties13);
    nonVisualGroupShapeProperties13.Append(applicationNonVisualDrawingProperties72);

    GroupShapeProperties groupShapeProperties13 = new GroupShapeProperties();

    A.TransformGroup transformGroup13 = new A.TransformGroup();
    A.Offset offset35 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents35 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset13 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents13 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup13.Append(offset35);
    transformGroup13.Append(extents35);
    transformGroup13.Append(childOffset13);
    transformGroup13.Append(childExtents13);

    groupShapeProperties13.Append(transformGroup13);

    Shape shape59 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties59 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties73 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)2U, Name = "Title 1"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties59 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks59 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties59.Append(shapeLocks59);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties73 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape59 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties73.Append(placeholderShape59);

    nonVisualShapeProperties59.Append(nonVisualDrawingProperties73);
    nonVisualShapeProperties59.Append(nonVisualShapeDrawingProperties59);
    nonVisualShapeProperties59.Append(applicationNonVisualDrawingProperties73);

    ShapeProperties shapeProperties60 = new ShapeProperties();

    A.Transform2D transform2D23 = new A.Transform2D();
    A.Offset offset36 = new A.Offset() {
      X = 1792288L, Y = 4800600L
    };
    A.Extents extents36 = new A.Extents() {
      Cx = 5486400L, Cy = 566738L
    };

    transform2D23.Append(offset36);
    transform2D23.Append(extents36);

    shapeProperties60.Append(transform2D23);

    TextBody textBody59 = new TextBody();
    A.BodyProperties bodyProperties59 = new A.BodyProperties() {
      Anchor = A.TextAnchoringTypeValues.Bottom
    };

    A.ListStyle listStyle59 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties21 = new A.Level1ParagraphProperties() {
      Alignment = A.TextAlignmentTypeValues.Left
    };
    A.DefaultRunProperties defaultRunProperties127 = new A.DefaultRunProperties() {
      FontSize = 2000, Bold = true
    };

    level1ParagraphProperties21.Append(defaultRunProperties127);

    listStyle59.Append(level1ParagraphProperties21);

    A.Paragraph paragraph95 = new A.Paragraph();

    A.Run run62 = new A.Run();

    A.RunProperties runProperties84 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties84.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text84 = new A.Text();
    text84.Text = "Click to edit Master title style";

    run62.Append(runProperties84);
    run62.Append(text84);
    A.EndParagraphRunProperties endParagraphRunProperties55 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph95.Append(run62);
    paragraph95.Append(endParagraphRunProperties55);

    textBody59.Append(bodyProperties59);
    textBody59.Append(listStyle59);
    textBody59.Append(paragraph95);

    shape59.Append(nonVisualShapeProperties59);
    shape59.Append(shapeProperties60);
    shape59.Append(textBody59);

    Shape shape60 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties60 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties74 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Picture Placeholder 2"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties60 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks60 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties60.Append(shapeLocks60);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties74 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape60 = new PlaceholderShape() {
      Type = PlaceholderValues.Picture, Index = (UInt32Value)1U
    };

    applicationNonVisualDrawingProperties74.Append(placeholderShape60);

    nonVisualShapeProperties60.Append(nonVisualDrawingProperties74);
    nonVisualShapeProperties60.Append(nonVisualShapeDrawingProperties60);
    nonVisualShapeProperties60.Append(applicationNonVisualDrawingProperties74);

    ShapeProperties shapeProperties61 = new ShapeProperties();

    A.Transform2D transform2D24 = new A.Transform2D();
    A.Offset offset37 = new A.Offset() {
      X = 1792288L, Y = 612775L
    };
    A.Extents extents37 = new A.Extents() {
      Cx = 5486400L, Cy = 4114800L
    };

    transform2D24.Append(offset37);
    transform2D24.Append(extents37);

    shapeProperties61.Append(transform2D24);

    TextBody textBody60 = new TextBody();
    A.BodyProperties bodyProperties60 = new A.BodyProperties();

    A.ListStyle listStyle60 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties22 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Indent = 0
    };
    A.NoBullet noBullet47 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties128 = new A.DefaultRunProperties() {
      FontSize = 3200
    };

    level1ParagraphProperties22.Append(noBullet47);
    level1ParagraphProperties22.Append(defaultRunProperties128);

    A.Level2ParagraphProperties level2ParagraphProperties14 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Indent = 0
    };
    A.NoBullet noBullet48 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties129 = new A.DefaultRunProperties() {
      FontSize = 2800
    };

    level2ParagraphProperties14.Append(noBullet48);
    level2ParagraphProperties14.Append(defaultRunProperties129);

    A.Level3ParagraphProperties level3ParagraphProperties14 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Indent = 0
    };
    A.NoBullet noBullet49 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties130 = new A.DefaultRunProperties() {
      FontSize = 2400
    };

    level3ParagraphProperties14.Append(noBullet49);
    level3ParagraphProperties14.Append(defaultRunProperties130);

    A.Level4ParagraphProperties level4ParagraphProperties14 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Indent = 0
    };
    A.NoBullet noBullet50 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties131 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level4ParagraphProperties14.Append(noBullet50);
    level4ParagraphProperties14.Append(defaultRunProperties131);

    A.Level5ParagraphProperties level5ParagraphProperties14 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Indent = 0
    };
    A.NoBullet noBullet51 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties132 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level5ParagraphProperties14.Append(noBullet51);
    level5ParagraphProperties14.Append(defaultRunProperties132);

    A.Level6ParagraphProperties level6ParagraphProperties14 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Indent = 0
    };
    A.NoBullet noBullet52 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties133 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level6ParagraphProperties14.Append(noBullet52);
    level6ParagraphProperties14.Append(defaultRunProperties133);

    A.Level7ParagraphProperties level7ParagraphProperties14 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Indent = 0
    };
    A.NoBullet noBullet53 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties134 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level7ParagraphProperties14.Append(noBullet53);
    level7ParagraphProperties14.Append(defaultRunProperties134);

    A.Level8ParagraphProperties level8ParagraphProperties14 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Indent = 0
    };
    A.NoBullet noBullet54 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties135 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level8ParagraphProperties14.Append(noBullet54);
    level8ParagraphProperties14.Append(defaultRunProperties135);

    A.Level9ParagraphProperties level9ParagraphProperties14 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Indent = 0
    };
    A.NoBullet noBullet55 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties136 = new A.DefaultRunProperties() {
      FontSize = 2000
    };

    level9ParagraphProperties14.Append(noBullet55);
    level9ParagraphProperties14.Append(defaultRunProperties136);

    listStyle60.Append(level1ParagraphProperties22);
    listStyle60.Append(level2ParagraphProperties14);
    listStyle60.Append(level3ParagraphProperties14);
    listStyle60.Append(level4ParagraphProperties14);
    listStyle60.Append(level5ParagraphProperties14);
    listStyle60.Append(level6ParagraphProperties14);
    listStyle60.Append(level7ParagraphProperties14);
    listStyle60.Append(level8ParagraphProperties14);
    listStyle60.Append(level9ParagraphProperties14);

    A.Paragraph paragraph96 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties56 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph96.Append(endParagraphRunProperties56);

    textBody60.Append(bodyProperties60);
    textBody60.Append(listStyle60);
    textBody60.Append(paragraph96);

    shape60.Append(nonVisualShapeProperties60);
    shape60.Append(shapeProperties61);
    shape60.Append(textBody60);

    Shape shape61 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties61 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties75 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U, Name = "Text Placeholder 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties61 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks61 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties61.Append(shapeLocks61);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties75 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape61 = new PlaceholderShape() {
      Type = PlaceholderValues.Body, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)2U
    };

    applicationNonVisualDrawingProperties75.Append(placeholderShape61);

    nonVisualShapeProperties61.Append(nonVisualDrawingProperties75);
    nonVisualShapeProperties61.Append(nonVisualShapeDrawingProperties61);
    nonVisualShapeProperties61.Append(applicationNonVisualDrawingProperties75);

    ShapeProperties shapeProperties62 = new ShapeProperties();

    A.Transform2D transform2D25 = new A.Transform2D();
    A.Offset offset38 = new A.Offset() {
      X = 1792288L, Y = 5367338L
    };
    A.Extents extents38 = new A.Extents() {
      Cx = 5486400L, Cy = 804862L
    };

    transform2D25.Append(offset38);
    transform2D25.Append(extents38);

    shapeProperties62.Append(transform2D25);

    TextBody textBody61 = new TextBody();
    A.BodyProperties bodyProperties61 = new A.BodyProperties();

    A.ListStyle listStyle61 = new A.ListStyle();

    A.Level1ParagraphProperties level1ParagraphProperties23 = new A.Level1ParagraphProperties() {
      LeftMargin = 0, Indent = 0
    };
    A.NoBullet noBullet56 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties137 = new A.DefaultRunProperties() {
      FontSize = 1400
    };

    level1ParagraphProperties23.Append(noBullet56);
    level1ParagraphProperties23.Append(defaultRunProperties137);

    A.Level2ParagraphProperties level2ParagraphProperties15 = new A.Level2ParagraphProperties() {
      LeftMargin = 457200, Indent = 0
    };
    A.NoBullet noBullet57 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties138 = new A.DefaultRunProperties() {
      FontSize = 1200
    };

    level2ParagraphProperties15.Append(noBullet57);
    level2ParagraphProperties15.Append(defaultRunProperties138);

    A.Level3ParagraphProperties level3ParagraphProperties15 = new A.Level3ParagraphProperties() {
      LeftMargin = 914400, Indent = 0
    };
    A.NoBullet noBullet58 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties139 = new A.DefaultRunProperties() {
      FontSize = 1000
    };

    level3ParagraphProperties15.Append(noBullet58);
    level3ParagraphProperties15.Append(defaultRunProperties139);

    A.Level4ParagraphProperties level4ParagraphProperties15 = new A.Level4ParagraphProperties() {
      LeftMargin = 1371600, Indent = 0
    };
    A.NoBullet noBullet59 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties140 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level4ParagraphProperties15.Append(noBullet59);
    level4ParagraphProperties15.Append(defaultRunProperties140);

    A.Level5ParagraphProperties level5ParagraphProperties15 = new A.Level5ParagraphProperties() {
      LeftMargin = 1828800, Indent = 0
    };
    A.NoBullet noBullet60 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties141 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level5ParagraphProperties15.Append(noBullet60);
    level5ParagraphProperties15.Append(defaultRunProperties141);

    A.Level6ParagraphProperties level6ParagraphProperties15 = new A.Level6ParagraphProperties() {
      LeftMargin = 2286000, Indent = 0
    };
    A.NoBullet noBullet61 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties142 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level6ParagraphProperties15.Append(noBullet61);
    level6ParagraphProperties15.Append(defaultRunProperties142);

    A.Level7ParagraphProperties level7ParagraphProperties15 = new A.Level7ParagraphProperties() {
      LeftMargin = 2743200, Indent = 0
    };
    A.NoBullet noBullet62 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties143 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level7ParagraphProperties15.Append(noBullet62);
    level7ParagraphProperties15.Append(defaultRunProperties143);

    A.Level8ParagraphProperties level8ParagraphProperties15 = new A.Level8ParagraphProperties() {
      LeftMargin = 3200400, Indent = 0
    };
    A.NoBullet noBullet63 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties144 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level8ParagraphProperties15.Append(noBullet63);
    level8ParagraphProperties15.Append(defaultRunProperties144);

    A.Level9ParagraphProperties level9ParagraphProperties15 = new A.Level9ParagraphProperties() {
      LeftMargin = 3657600, Indent = 0
    };
    A.NoBullet noBullet64 = new A.NoBullet();
    A.DefaultRunProperties defaultRunProperties145 = new A.DefaultRunProperties() {
      FontSize = 900
    };

    level9ParagraphProperties15.Append(noBullet64);
    level9ParagraphProperties15.Append(defaultRunProperties145);

    listStyle61.Append(level1ParagraphProperties23);
    listStyle61.Append(level2ParagraphProperties15);
    listStyle61.Append(level3ParagraphProperties15);
    listStyle61.Append(level4ParagraphProperties15);
    listStyle61.Append(level5ParagraphProperties15);
    listStyle61.Append(level6ParagraphProperties15);
    listStyle61.Append(level7ParagraphProperties15);
    listStyle61.Append(level8ParagraphProperties15);
    listStyle61.Append(level9ParagraphProperties15);

    A.Paragraph paragraph97 = new A.Paragraph();
    A.ParagraphProperties paragraphProperties72 = new A.ParagraphProperties() {
      Level = 0
    };

    A.Run run63 = new A.Run();

    A.RunProperties runProperties85 = new A.RunProperties() {
      Language = "en-US"
    };
    runProperties85.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text85 = new A.Text();
    text85.Text = "Click to edit Master text styles";

    run63.Append(runProperties85);
    run63.Append(text85);

    paragraph97.Append(paragraphProperties72);
    paragraph97.Append(run63);

    textBody61.Append(bodyProperties61);
    textBody61.Append(listStyle61);
    textBody61.Append(paragraph97);

    shape61.Append(nonVisualShapeProperties61);
    shape61.Append(shapeProperties62);
    shape61.Append(textBody61);

    Shape shape62 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties62 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties76 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)5U, Name = "Date Placeholder 4"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties62 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks62 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties62.Append(shapeLocks62);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties76 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape62 = new PlaceholderShape() {
      Type = PlaceholderValues.DateAndTime, Size = PlaceholderSizeValues.Half, Index = (UInt32Value)10U
    };

    applicationNonVisualDrawingProperties76.Append(placeholderShape62);

    nonVisualShapeProperties62.Append(nonVisualDrawingProperties76);
    nonVisualShapeProperties62.Append(nonVisualShapeDrawingProperties62);
    nonVisualShapeProperties62.Append(applicationNonVisualDrawingProperties76);
    ShapeProperties shapeProperties63 = new ShapeProperties();

    TextBody textBody62 = new TextBody();
    A.BodyProperties bodyProperties62 = new A.BodyProperties();
    A.ListStyle listStyle62 = new A.ListStyle();

    A.Paragraph paragraph98 = new A.Paragraph();

    A.Field field23 = new A.Field() {
      Id = "{C43387D2-8DDA-4AB3-AC91-BA9E2F860BED}", Type = "datetimeFigureOut"
    };

    A.RunProperties runProperties86 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties86.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties73 = new A.ParagraphProperties();
    A.Text text86 = new A.Text();
    text86.Text = "20.11.2013";

    field23.Append(runProperties86);
    field23.Append(paragraphProperties73);
    field23.Append(text86);
    A.EndParagraphRunProperties endParagraphRunProperties57 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph98.Append(field23);
    paragraph98.Append(endParagraphRunProperties57);

    textBody62.Append(bodyProperties62);
    textBody62.Append(listStyle62);
    textBody62.Append(paragraph98);

    shape62.Append(nonVisualShapeProperties62);
    shape62.Append(shapeProperties63);
    shape62.Append(textBody62);

    Shape shape63 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties63 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties77 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)6U, Name = "Footer Placeholder 5"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties63 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks63 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties63.Append(shapeLocks63);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties77 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape63 = new PlaceholderShape() {
      Type = PlaceholderValues.Footer, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)11U
    };

    applicationNonVisualDrawingProperties77.Append(placeholderShape63);

    nonVisualShapeProperties63.Append(nonVisualDrawingProperties77);
    nonVisualShapeProperties63.Append(nonVisualShapeDrawingProperties63);
    nonVisualShapeProperties63.Append(applicationNonVisualDrawingProperties77);
    ShapeProperties shapeProperties64 = new ShapeProperties();

    TextBody textBody63 = new TextBody();
    A.BodyProperties bodyProperties63 = new A.BodyProperties();
    A.ListStyle listStyle63 = new A.ListStyle();

    A.Paragraph paragraph99 = new A.Paragraph();
    A.EndParagraphRunProperties endParagraphRunProperties58 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph99.Append(endParagraphRunProperties58);

    textBody63.Append(bodyProperties63);
    textBody63.Append(listStyle63);
    textBody63.Append(paragraph99);

    shape63.Append(nonVisualShapeProperties63);
    shape63.Append(shapeProperties64);
    shape63.Append(textBody63);

    Shape shape64 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties64 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties78 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)7U, Name = "Slide Number Placeholder 6"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties64 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks64 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties64.Append(shapeLocks64);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties78 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape64 = new PlaceholderShape() {
      Type = PlaceholderValues.SlideNumber, Size = PlaceholderSizeValues.Quarter, Index = (UInt32Value)12U
    };

    applicationNonVisualDrawingProperties78.Append(placeholderShape64);

    nonVisualShapeProperties64.Append(nonVisualDrawingProperties78);
    nonVisualShapeProperties64.Append(nonVisualShapeDrawingProperties64);
    nonVisualShapeProperties64.Append(applicationNonVisualDrawingProperties78);
    ShapeProperties shapeProperties65 = new ShapeProperties();

    TextBody textBody64 = new TextBody();
    A.BodyProperties bodyProperties64 = new A.BodyProperties();
    A.ListStyle listStyle64 = new A.ListStyle();

    A.Paragraph paragraph100 = new A.Paragraph();

    A.Field field24 = new A.Field() {
      Id = "{F3E77317-3B17-4293-A77F-D83F50900F68}", Type = "slidenum"
    };

    A.RunProperties runProperties87 = new A.RunProperties() {
      Language = "ru-RU"
    };
    runProperties87.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.ParagraphProperties paragraphProperties74 = new A.ParagraphProperties();
    A.Text text87 = new A.Text();
    text87.Text = "‹#›";

    field24.Append(runProperties87);
    field24.Append(paragraphProperties74);
    field24.Append(text87);
    A.EndParagraphRunProperties endParagraphRunProperties59 = new A.EndParagraphRunProperties() {
      Language = "ru-RU"
    };

    paragraph100.Append(field24);
    paragraph100.Append(endParagraphRunProperties59);

    textBody64.Append(bodyProperties64);
    textBody64.Append(listStyle64);
    textBody64.Append(paragraph100);

    shape64.Append(nonVisualShapeProperties64);
    shape64.Append(shapeProperties65);
    shape64.Append(textBody64);

    shapeTree13.Append(nonVisualGroupShapeProperties13);
    shapeTree13.Append(groupShapeProperties13);
    shapeTree13.Append(shape59);
    shapeTree13.Append(shape60);
    shapeTree13.Append(shape61);
    shapeTree13.Append(shape62);
    shapeTree13.Append(shape63);
    shapeTree13.Append(shape64);

    commonSlideData13.Append(shapeTree13);

    ColorMapOverride colorMapOverride12 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping12 = new A.MasterColorMapping();

    colorMapOverride12.Append(masterColorMapping12);

    slideLayout11.Append(commonSlideData13);
    slideLayout11.Append(colorMapOverride12);

    slideLayoutPart11.SlideLayout = slideLayout11;
  }

  // Generates content of viewPropertiesPart1.
  private void GenerateViewPropertiesPart1Content(ViewPropertiesPart viewPropertiesPart1) {
    ViewProperties viewProperties1 = new ViewProperties();
    viewProperties1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    viewProperties1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    viewProperties1.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    NormalViewProperties normalViewProperties1 = new NormalViewProperties() {
      ShowOutlineIcons = false
    };
    RestoredLeft restoredLeft1 = new RestoredLeft() {
      Size = 15651, AutoAdjust = false
    };
    RestoredTop restoredTop1 = new RestoredTop() {
      Size = 94653, AutoAdjust = false
    };

    normalViewProperties1.Append(restoredLeft1);
    normalViewProperties1.Append(restoredTop1);

    SlideViewProperties slideViewProperties1 = new SlideViewProperties();

    CommonSlideViewProperties commonSlideViewProperties1 = new CommonSlideViewProperties();

    CommonViewProperties commonViewProperties1 = new CommonViewProperties() {
      VariableScale = true
    };

    ScaleFactor scaleFactor1 = new ScaleFactor();
    A.ScaleX scaleX1 = new A.ScaleX() {
      Numerator = 111, Denominator = 100
    };
    A.ScaleY scaleY1 = new A.ScaleY() {
      Numerator = 111, Denominator = 100
    };

    scaleFactor1.Append(scaleX1);
    scaleFactor1.Append(scaleY1);
    Origin origin1 = new Origin() {
      X = -768L, Y = -90L
    };

    commonViewProperties1.Append(scaleFactor1);
    commonViewProperties1.Append(origin1);

    GuideList guideList1 = new GuideList();
    Guide guide1 = new Guide() {
      Orientation = DirectionValues.Horizontal, Position = 2160
    };
    Guide guide2 = new Guide() {
      Position = 2880
    };

    guideList1.Append(guide1);
    guideList1.Append(guide2);

    commonSlideViewProperties1.Append(commonViewProperties1);
    commonSlideViewProperties1.Append(guideList1);

    slideViewProperties1.Append(commonSlideViewProperties1);

    OutlineViewProperties outlineViewProperties1 = new OutlineViewProperties();

    CommonViewProperties commonViewProperties2 = new CommonViewProperties();

    ScaleFactor scaleFactor2 = new ScaleFactor();
    A.ScaleX scaleX2 = new A.ScaleX() {
      Numerator = 33, Denominator = 100
    };
    A.ScaleY scaleY2 = new A.ScaleY() {
      Numerator = 33, Denominator = 100
    };

    scaleFactor2.Append(scaleX2);
    scaleFactor2.Append(scaleY2);
    Origin origin2 = new Origin() {
      X = 0L, Y = 0L
    };

    commonViewProperties2.Append(scaleFactor2);
    commonViewProperties2.Append(origin2);

    outlineViewProperties1.Append(commonViewProperties2);

    NotesTextViewProperties notesTextViewProperties1 = new NotesTextViewProperties();

    CommonViewProperties commonViewProperties3 = new CommonViewProperties();

    ScaleFactor scaleFactor3 = new ScaleFactor();
    A.ScaleX scaleX3 = new A.ScaleX() {
      Numerator = 100, Denominator = 100
    };
    A.ScaleY scaleY3 = new A.ScaleY() {
      Numerator = 100, Denominator = 100
    };

    scaleFactor3.Append(scaleX3);
    scaleFactor3.Append(scaleY3);
    Origin origin3 = new Origin() {
      X = 0L, Y = 0L
    };

    commonViewProperties3.Append(scaleFactor3);
    commonViewProperties3.Append(origin3);

    notesTextViewProperties1.Append(commonViewProperties3);
    GridSpacing gridSpacing1 = new GridSpacing() {
      Cx = 73736200L, Cy = 73736200L
    };

    viewProperties1.Append(normalViewProperties1);
    viewProperties1.Append(slideViewProperties1);
    viewProperties1.Append(outlineViewProperties1);
    viewProperties1.Append(notesTextViewProperties1);
    viewProperties1.Append(gridSpacing1);

    viewPropertiesPart1.ViewProperties = viewProperties1;
  }

  // Generates content of presentationPropertiesPart1.
  private void GeneratePresentationPropertiesPart1Content(PresentationPropertiesPart presentationPropertiesPart1) {
    PresentationProperties presentationProperties1 = new PresentationProperties();
    presentationProperties1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    presentationProperties1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    presentationProperties1.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    presentationPropertiesPart1.PresentationProperties = presentationProperties1;
  }

  private void GenerateSlidePartContent(SlidePart slidePart, int idx, PageInfo title) {
    Slide slide = new Slide();
    slide.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
    slide.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    slide.AddNamespaceDeclaration("p", "http://schemas.openxmlformats.org/presentationml/2006/main");

    CommonSlideData commonSlideData = new CommonSlideData();

    ShapeTree shapeTree = new ShapeTree();

    NonVisualGroupShapeProperties nonVisualGroupShapeProperties15 = new NonVisualGroupShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties82 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)1U, Name = ""
    };
    NonVisualGroupShapeDrawingProperties nonVisualGroupShapeDrawingProperties15 = new NonVisualGroupShapeDrawingProperties();
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties82 = new ApplicationNonVisualDrawingProperties();

    nonVisualGroupShapeProperties15.Append(nonVisualDrawingProperties82);
    nonVisualGroupShapeProperties15.Append(nonVisualGroupShapeDrawingProperties15);
    nonVisualGroupShapeProperties15.Append(applicationNonVisualDrawingProperties82);

    GroupShapeProperties groupShapeProperties15 = new GroupShapeProperties();

    A.TransformGroup transformGroup15 = new A.TransformGroup();
    A.Offset offset42 = new A.Offset() {
      X = 0L, Y = 0L
    };
    A.Extents extents42 = new A.Extents() {
      Cx = 0L, Cy = 0L
    };
    A.ChildOffset childOffset15 = new A.ChildOffset() {
      X = 0L, Y = 0L
    };
    A.ChildExtents childExtents15 = new A.ChildExtents() {
      Cx = 0L, Cy = 0L
    };

    transformGroup15.Append(offset42);
    transformGroup15.Append(extents42);
    transformGroup15.Append(childOffset15);
    transformGroup15.Append(childExtents15);

    groupShapeProperties15.Append(transformGroup15);
    // TITLE
    Shape shape66 = new Shape();

    NonVisualShapeProperties nonVisualShapeProperties66 = new NonVisualShapeProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties83 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)3U, Name = "Title 3"
    };

    NonVisualShapeDrawingProperties nonVisualShapeDrawingProperties66 = new NonVisualShapeDrawingProperties();
    A.ShapeLocks shapeLocks66 = new A.ShapeLocks() {
      NoGrouping = true
    };

    nonVisualShapeDrawingProperties66.Append(shapeLocks66);

    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties83 = new ApplicationNonVisualDrawingProperties();
    PlaceholderShape placeholderShape66 = new PlaceholderShape() {
      Type = PlaceholderValues.Title
    };

    applicationNonVisualDrawingProperties83.Append(placeholderShape66);

    nonVisualShapeProperties66.Append(nonVisualDrawingProperties83);
    nonVisualShapeProperties66.Append(nonVisualShapeDrawingProperties66);
    nonVisualShapeProperties66.Append(applicationNonVisualDrawingProperties83);

    ShapeProperties shapeProperties68 = new ShapeProperties();

    A.Transform2D transform2D28 = new A.Transform2D();
    A.Offset offset43 = new A.Offset() {
      X = 457200L, Y = 274638L
    };
    A.Extents extents43 = new A.Extents() {
      Cx = 8229600L, Cy = 296842L
    };

    transform2D28.Append(offset43);
    transform2D28.Append(extents43);

    shapeProperties68.Append(transform2D28);

    TextBody textBody66 = new TextBody();

    A.BodyProperties bodyProperties66 = new A.BodyProperties();
    A.NoAutoFit noAutoFit3 = new A.NoAutoFit();

    bodyProperties66.Append(noAutoFit3);
    A.ListStyle listStyle66 = new A.ListStyle();

    A.Paragraph paragraph102 = new A.Paragraph();

    A.Run run65 = new A.Run();

    A.RunProperties runProperties89 = new A.RunProperties() {
      Language = "en-US", FontSize = 1400, Dirty = false
    };
    runProperties89.SetAttribute(new OpenXmlAttribute("", "smtClean", "", "0"));
    A.Text text89 = new A.Text();
    text89.Text = title.Title;

    run65.Append(runProperties89);
    run65.Append(text89);
    A.EndParagraphRunProperties endParagraphRunProperties61 = new A.EndParagraphRunProperties() {
      Language = "ru-RU", FontSize = 1400, Dirty = false
    };

    paragraph102.Append(run65);
    paragraph102.Append(endParagraphRunProperties61);

    textBody66.Append(bodyProperties66);
    textBody66.Append(listStyle66);
    textBody66.Append(paragraph102);

    shape66.Append(nonVisualShapeProperties66);
    shape66.Append(shapeProperties68);
    shape66.Append(textBody66);

    Picture picture3 = new Picture();

    NonVisualPictureProperties nonVisualPictureProperties3 = new NonVisualPictureProperties();
    NonVisualDrawingProperties nonVisualDrawingProperties84 = new NonVisualDrawingProperties() {
      Id = (UInt32Value)4U,
      Name = "Picture " + (idx + 2),
      Description = "Picture" + (idx + 2) + ".jpg"
    };

    NonVisualPictureDrawingProperties nonVisualPictureDrawingProperties3 = new NonVisualPictureDrawingProperties();
    A.PictureLocks pictureLocks3 = new A.PictureLocks();

    nonVisualPictureDrawingProperties3.Append(pictureLocks3);
    ApplicationNonVisualDrawingProperties applicationNonVisualDrawingProperties84 = new ApplicationNonVisualDrawingProperties();

    nonVisualPictureProperties3.Append(nonVisualDrawingProperties84);
    nonVisualPictureProperties3.Append(nonVisualPictureDrawingProperties3);
    nonVisualPictureProperties3.Append(applicationNonVisualDrawingProperties84);

    BlipFill blipFill3 = new BlipFill();
    A.Blip blip3 = new A.Blip() {
      Embed = idx == 0 ? "rId2" : ("rrId" + (idx + 2))
    };

    A.Stretch stretch3 = new A.Stretch();
    A.FillRectangle fillRectangle3 = new A.FillRectangle();
    stretch3.Append(fillRectangle3);

    blipFill3.Append(blip3);
    blipFill3.Append(stretch3);

    ShapeProperties shapeProperties69 = new ShapeProperties();

    A.Transform2D transform2D29 = new A.Transform2D();
    /*A.Offset offset44 = new A.Offset() { X = 523835L, Y = 690567L };
    A.Extents extents44 = new A.Extents() { Cx = 8096250L, Cy = 5476875L };*/
    A.Offset offset44 = new A.Offset() {
      X = 500000L, Y = 500000L
    };
    A.Extents extents44;
    if (title.IsChart) {
      extents44 = new A.Extents() {
        Cx = 8100000L, Cy = 3720000L
      };
    } else {
      extents44 = new A.Extents() {
        Cx = 8300000L, Cy = 5400000L
      };
    }


    transform2D29.Append(offset44);
    transform2D29.Append(extents44);

    A.PresetGeometry presetGeometry8 = new A.PresetGeometry() {
      Preset = A.ShapeTypeValues.Rectangle
    };
    A.AdjustValueList adjustValueList8 = new A.AdjustValueList();

    presetGeometry8.Append(adjustValueList8);

    shapeProperties69.Append(transform2D29);
    shapeProperties69.Append(presetGeometry8);

    picture3.Append(nonVisualPictureProperties3);
    picture3.Append(blipFill3);
    picture3.Append(shapeProperties69);

    shapeTree.Append(nonVisualGroupShapeProperties15);
    shapeTree.Append(groupShapeProperties15);
    shapeTree.Append(shape66);
    shapeTree.Append(picture3);

    commonSlideData.Append(shapeTree);

    ColorMapOverride colorMapOverride14 = new ColorMapOverride();
    A.MasterColorMapping masterColorMapping14 = new A.MasterColorMapping();

    colorMapOverride14.Append(masterColorMapping14);

    slide.Append(commonSlideData);
    slide.Append(colorMapOverride14);

    slidePart.Slide = slide;
  }

  // Generates content of extendedFilePropertiesPart1.
  private void GenerateExtendedFilePropertiesPart1Content(ExtendedFilePropertiesPart extendedFilePropertiesPart1) {
    Ap.Properties properties1 = new Ap.Properties();
    properties1.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
    Ap.TotalTime totalTime1 = new Ap.TotalTime();
    totalTime1.Text = "15";
    Ap.Words words1 = new Ap.Words();
    words1.Text = "12";
    Ap.Application application1 = new Ap.Application();
    application1.Text = "Microsoft Office PowerPoint";
    Ap.PresentationFormat presentationFormat1 = new Ap.PresentationFormat();
    presentationFormat1.Text = "On-screen Show (4:3)";
    Ap.Paragraphs paragraphs1 = new Ap.Paragraphs();
    paragraphs1.Text = "3";
    Ap.Slides slides1 = new Ap.Slides();
    slides1.Text = "3";
    Ap.Notes notes1 = new Ap.Notes();
    notes1.Text = "0";
    Ap.HiddenSlides hiddenSlides1 = new Ap.HiddenSlides();
    hiddenSlides1.Text = "0";
    Ap.MultimediaClips multimediaClips1 = new Ap.MultimediaClips();
    multimediaClips1.Text = "0";
    Ap.ScaleCrop scaleCrop1 = new Ap.ScaleCrop();
    scaleCrop1.Text = "false";

    Ap.HeadingPairs headingPairs1 = new Ap.HeadingPairs();

    Vt.VTVector vTVector1 = new Vt.VTVector() {
      BaseType = Vt.VectorBaseValues.Variant, Size = (UInt32Value)4U
    };

    Vt.Variant variant1 = new Vt.Variant();
    Vt.VTLPSTR vTLPSTR1 = new Vt.VTLPSTR();
    vTLPSTR1.Text = "Theme";

    variant1.Append(vTLPSTR1);

    Vt.Variant variant2 = new Vt.Variant();
    Vt.VTInt32 vTInt321 = new Vt.VTInt32();
    vTInt321.Text = "1";

    variant2.Append(vTInt321);

    Vt.Variant variant3 = new Vt.Variant();
    Vt.VTLPSTR vTLPSTR2 = new Vt.VTLPSTR();
    vTLPSTR2.Text = "Slide Titles";

    variant3.Append(vTLPSTR2);

    Vt.Variant variant4 = new Vt.Variant();
    Vt.VTInt32 vTInt322 = new Vt.VTInt32();
    vTInt322.Text = "3";

    variant4.Append(vTInt322);

    vTVector1.Append(variant1);
    vTVector1.Append(variant2);
    vTVector1.Append(variant3);
    vTVector1.Append(variant4);

    headingPairs1.Append(vTVector1);

    Ap.TitlesOfParts titlesOfParts1 = new Ap.TitlesOfParts();

    Vt.VTVector vTVector2 = new Vt.VTVector() {
      BaseType = Vt.VectorBaseValues.Lpstr, Size = (UInt32Value)4U
    };
    Vt.VTLPSTR vTLPSTR3 = new Vt.VTLPSTR();
    vTLPSTR3.Text = "Office Theme";
    Vt.VTLPSTR vTLPSTR4 = new Vt.VTLPSTR();
    vTLPSTR4.Text = "Sample title TTT 1";
    Vt.VTLPSTR vTLPSTR5 = new Vt.VTLPSTR();
    vTLPSTR5.Text = "Sample title TTT 2";
    Vt.VTLPSTR vTLPSTR6 = new Vt.VTLPSTR();
    vTLPSTR6.Text = "Sample title TTT 3";

    vTVector2.Append(vTLPSTR3);
    vTVector2.Append(vTLPSTR4);
    vTVector2.Append(vTLPSTR5);
    vTVector2.Append(vTLPSTR6);

    titlesOfParts1.Append(vTVector2);
    Ap.LinksUpToDate linksUpToDate1 = new Ap.LinksUpToDate();
    linksUpToDate1.Text = "false";
    Ap.SharedDocument sharedDocument1 = new Ap.SharedDocument();
    sharedDocument1.Text = "false";
    Ap.HyperlinksChanged hyperlinksChanged1 = new Ap.HyperlinksChanged();
    hyperlinksChanged1.Text = "false";
    Ap.ApplicationVersion applicationVersion1 = new Ap.ApplicationVersion();
    applicationVersion1.Text = "12.0000";

    properties1.Append(totalTime1);
    properties1.Append(words1);
    properties1.Append(application1);
    properties1.Append(presentationFormat1);
    properties1.Append(paragraphs1);
    properties1.Append(slides1);
    properties1.Append(notes1);
    properties1.Append(hiddenSlides1);
    properties1.Append(multimediaClips1);
    properties1.Append(scaleCrop1);
    properties1.Append(headingPairs1);
    properties1.Append(titlesOfParts1);
    properties1.Append(linksUpToDate1);
    properties1.Append(sharedDocument1);
    properties1.Append(hyperlinksChanged1);
    properties1.Append(applicationVersion1);

    extendedFilePropertiesPart1.Properties = properties1;
  }

  private void SetPackageProperties(OpenXmlPackage document) {
    document.PackageProperties.Creator = "Izenda";
    document.PackageProperties.Title = "Slide 1";
    document.PackageProperties.Revision = "4";
    document.PackageProperties.Created = System.Xml.XmlConvert.ToDateTime("2013-11-20T12:15:53Z", System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
    document.PackageProperties.Modified = System.Xml.XmlConvert.ToDateTime("2013-11-20T12:48:03Z", System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
    document.PackageProperties.LastModifiedBy = "Izenda";
  }

  #region Binary Data
  private string thumbnailPart1Data = "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";

  private System.IO.Stream GetBinaryDataStream(string base64String) {
    return new System.IO.MemoryStream(System.Convert.FromBase64String(base64String));
  }

  #endregion
}
