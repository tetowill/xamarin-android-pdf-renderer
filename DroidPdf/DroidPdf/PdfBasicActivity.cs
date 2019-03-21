using System;
using Debug = System.Diagnostics.Debug;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Content.PM;

namespace DroidPdf
{
	//[Activity(Label = "PdfBasicActivity", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
	public class PdfBasicActivity : Activity
	{
		// RecyclerView instance that displays the pdf file:
		RecyclerView mRecyclerView;

		// Layout manager that lays out each card in the RecyclerView:
		RecyclerView.LayoutManager mLayoutManager;

		// Adapter that accesses the data set (a pdf file):
		PdfBitmapAdapter mAdapter;

		readonly string fileName = "837pages.pdf";

		List<Bitmap> pages;
		Bitmap bitmap;
		PdfRenderer.Page page;
		int pageCount;
		bool lowMemory;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource:
			SetContentView(Resource.Layout.pdf_activity);

			// Get our RecyclerView layout:
			mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

			// Use the built-in linear layout manager:
			mLayoutManager = new LinearLayoutManager(this);

			// Plug the layout manager into the RecyclerView:
			mRecyclerView.SetLayoutManager(mLayoutManager);

			pages = new List<Bitmap>();
			RenderPages();
		}

		void RenderPages()
		{
			Stream inputStream = Assets.Open(fileName);
			using (var outputStream = this.OpenFileOutput("_sample.pdf", Android.Content.FileCreationMode.Private))
			{
				inputStream.CopyTo(outputStream);
			}
			var fileStreamPath = this.GetFileStreamPath("_sample.pdf");
			MemoryStream m_memoryStream = new MemoryStream();
			File.OpenRead(fileStreamPath.AbsolutePath).CopyTo(m_memoryStream);

			var renderer = new PdfRenderer(ParcelFileDescriptor.Open(fileStreamPath, ParcelFileMode.ReadOnly));
			var screenWidth = Resources.DisplayMetrics.WidthPixels;

			// render all pages
			pageCount = renderer.PageCount;
			for (int i = 0; i < pageCount; i++)
			{
				page = renderer.OpenPage(i);

				// create bitmap at appropriate size
				var ratio = (float)page.Height / page.Width;
				var newHeight = screenWidth * ratio;
				bitmap = Bitmap.CreateBitmap(screenWidth, (int)newHeight, Bitmap.Config.Argb8888);

				// render PDF page to bitmap
				page.Render(bitmap, null, null, PdfRenderMode.ForDisplay);

				// add bitmap to list
				pages.Add(bitmap);

				// close the page
				page.Close();

				// if free memory is less than the size of two page bitmaps and we still have pages left to load 
				// we'll stop loading and then display a message about downloading the full doc
				// * this catch doesn't seem to work on Android O but it worked on a Nexus 5 running Android M, apparently Android O changes some memory handling
				Debug.WriteLine("\nMemory usage " + i + "\n" + "Bitmap Byte Count: " + bitmap.ByteCount + "\nMemory Available: " + MemoryAvailable());
				if (bitmap.ByteCount * 2 > MemoryAvailable() && i < pageCount - 1)
				{
					lowMemory = true;
					break;
				}
			}

			// close the renderer
			renderer.Close();

			// Create an adapter for the RecyclerView, and pass it the
			// data set (the bitmap list) to manage:
			mAdapter = new PdfBitmapAdapter(pages);

			// Register the item click handler (below) with the adapter:
			mAdapter.ItemClick += OnItemClick;

			// Plug the adapter into the RecyclerView:
			mRecyclerView.SetAdapter(mAdapter);

			if (lowMemory)
			{
				try
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Out Of Memory");
					alert.SetMessage("You are only viewing " + pages.Count + " out of " + pageCount + " pages.");
					alert.SetPositiveButton("OK", (senderAlert, args) => { });
					Dialog dialog = alert.Create();
					dialog.Show();
				}
				catch (Exception e)
				{
					Debug.WriteLine("Exception: " + e.Message);
				}
			}
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

		long MemoryAvailable()
		{
			long memoryUsed = Java.Lang.Runtime.GetRuntime().TotalMemory() - Java.Lang.Runtime.GetRuntime().FreeMemory();
			long memoryAvailable = Java.Lang.Runtime.GetRuntime().MaxMemory() - memoryUsed;
			return memoryAvailable;
		}
	}
}
