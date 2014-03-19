To use Annotations please create an Annotations table with the following structure:

CREATE TABLE [dbo].[Annotations](
  [ReportSet] [nvarchar](4000) NULL,
  [ColName] [nvarchar](4000) NULL,
  [RowNumber] [int] NULL,
  [CellData] [nvarchar](4000) NULL,
  [Value] [nvarchar](4000) NULL,
  [TenantId] [nvarchar](250) NULL
) ON [PRIMARY]


Next, set the Connection String in Annotations.aspx.cs where it says "INSERT_CONNECTION_STRING_HERE" to the Database with the Annotations table
