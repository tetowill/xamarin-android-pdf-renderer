using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.IO;
using DroidPdf;
using Android.Content.PM;
using DroidPdf.Helpers;

namespace RecyclerViewer
{
	[Activity(Label = "PdfFileActivity", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
	public class PdfFileActivity : Activity
	{
		// RecyclerView instance that displays the pdf file:
		RecyclerView mRecyclerView;

		// Layout manager that lays out each card in the RecyclerView:
		RecyclerView.LayoutManager mLayoutManager;

		// Adapter that accesses the data set (a pdf file):
		PdfFileAdapter mAdapter;

		// pdf file that is managed by the adapter:
		PdfFile mPdfFile;

		readonly string fileName = "837pages.pdf";

		#region PDF Byte[]
		//string filePathTemp;
		//FileStream fileStream;
		//Java.IO.File file;
		#endregion

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource:
			SetContentView(Resource.Layout.pdf_activity);

			#region PDF Local Asset
			Stream inputStream = Assets.Open(fileName);
			using (var outputStream = this.OpenFileOutput("_sample.pdf", Android.Content.FileCreationMode.Private))
			{
				inputStream.CopyTo(outputStream);
			}
			var fileStreamPath = this.GetFileStreamPath("_sample.pdf");
			MemoryStream m_memoryStream = new MemoryStream();
			File.OpenRead(fileStreamPath.AbsolutePath).CopyTo(m_memoryStream);

			// Instantiate the pdf file:
			mPdfFile = new PdfFile
			{
				ScreenWidth = Resources.DisplayMetrics.WidthPixels
			};
			mPdfFile.RenderPDFPagesIntoImages(fileStreamPath);
			#endregion

			#region PDF Byte[]
			//byte[] pdfData = null;

			//// get the PDF as a byte[] from your API however you'd like
			//pdfData = await GetPdfAsByteArray();

			//if (pdfData != null)
			//{
			//	string directory = GetExternalFilesDir(null).ToString();
			//	filePathTemp = System.IO.Path.Combine(directory, "fileName");

			//	fileStream = System.IO.File.Create(filePathTemp);
			//	fileStream.Write(pdfData, 0, pdfData.Length);
			//	fileStream.Close();

			//	file = new Java.IO.File(filePathTemp);

			// Instantiate the pdf file:
			//	mPdfFile = new PdfFile
			//	{
			//		ScreenWidth = Resources.DisplayMetrics.WidthPixels
			//	};
			//	mPdfFile.RenderPDFPagesIntoImages(file);
			//}
			#endregion

			// Get our RecyclerView layout:
			mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

			// Use the built-in linear layout manager:
			mLayoutManager = new LinearLayoutManager(this);

			// Plug the layout manager into the RecyclerView:
			mRecyclerView.SetLayoutManager(mLayoutManager);

			// Create an adapter for the RecyclerView, and pass it the
			// data set (the pdf file) to manage:
			mAdapter = new PdfFileAdapter(mPdfFile);

			// Register the item click handler (below) with the adapter:
			mAdapter.ItemClick += OnItemClick;

			// Plug the adapter into the RecyclerView:
			mRecyclerView.SetAdapter(mAdapter);
		}

		protected override void OnResume()
		{
			base.OnResume();

			// set orientation to portrait
			// If you start a portrait forced activity while holding your device in landscape mode, 
			// the activity will be recreated (destroyed and created again following the activity lifecycle).
			this.RequestedOrientation = ScreenOrientation.Portrait;
		}

		// Handler for the item click event:
		void OnItemClick(object sender, int position)
		{
			// Display a toast that briefly shows the enumeration of the selected pdf page:
			int pageNum = position + 1;
			Toast.MakeText(this, "This is pdf page number " + pageNum, ToastLength.Short).Show();
		}
	}
}