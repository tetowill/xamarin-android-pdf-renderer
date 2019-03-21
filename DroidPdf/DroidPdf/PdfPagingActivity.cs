using System;
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
	//[Activity(Label = "PdfPagingActivity", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
	public class PdfPagingActivity : Activity
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
		int pageCurrent;
		readonly int pageMax = 5;
		int pageGroupCount;
		int screenWidth;
		Button btnPrev;
		Button btnNext;
		TextView textPageCurrent;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource:
			SetContentView(Resource.Layout.pdf_paging_activity);

			// Get our RecyclerView layout:
			mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

			btnPrev = FindViewById<Button>(Resource.Id.button_prev);
			btnPrev.Text = "Prev";
			btnPrev.Click += BtnPrev_Click;

			btnNext = FindViewById<Button>(Resource.Id.button_next);
			btnNext.Text = "Next";
			btnNext.Click += BtnNext_Click;

			textPageCurrent = FindViewById<TextView>(Resource.Id.text_page_current);

			// Use the built-in linear layout manager:
			mLayoutManager = new LinearLayoutManager(this);

			// Plug the layout manager into the RecyclerView:
			mRecyclerView.SetLayoutManager(mLayoutManager);

			// Get screen width for bitmap sizing
			screenWidth = Resources.DisplayMetrics.WidthPixels;

			// Set up pdf bitmap adapter
			pages = new List<Bitmap>();
			mAdapter = new PdfBitmapAdapter(pages);
			mRecyclerView.SetAdapter(mAdapter);

			// Register the item click handler (below) with the adapter:
			mAdapter.ItemClick += OnItemClick;

			RenderPages();
		}

		void RenderPages(bool next = true)
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

			ClearPages();
			pages = new List<Bitmap>();

			if (pageCount == 0)
			{
				pageCount = renderer.PageCount;
			}

			bool goingBackFromEnd = (pageCount == pageCurrent && pageGroupCount < pageMax && !next);

			// set starting page number
			int pageGroupStart;
			if (next)
			{
				pageGroupStart = pageCurrent;
			}
			else if (goingBackFromEnd)
			{
				// if we're at the end and going back we need to adjust the page group start by the previous group count instead of the maximum
				pageGroupStart = pageCurrent - pageGroupCount;
				// we also need to update the current page here instead of in the loop so it stays on track
				pageCurrent = pageGroupStart;
			}
			else
			{
				pageGroupStart = pageCurrent - pageMax;
			}

			// get next paging group count
			int pagesLeft = pageCount - pageCurrent;
			if (pagesLeft < pageMax && next)
			{
				// use pages left if that's below the maximum
				pageGroupCount = pagesLeft;
			}
			else
			{
				pageGroupCount = pageMax;
			}

			// render pages
			for (int i = 0; i < pageGroupCount; i++)
			{
				if (next)
				{
					page = renderer.OpenPage(pageGroupStart + i);
					pageCurrent++;
				}
				else
				{
					page = renderer.OpenPage(pageGroupStart - (i + 1));
					if (!goingBackFromEnd)
					{
						pageCurrent--;
					}
				}

				// create bitmap of page
				var ratio = (float)page.Height / page.Width;
				var newHeight = screenWidth * ratio;
				bitmap = Bitmap.CreateBitmap(screenWidth, (int)newHeight, Bitmap.Config.Argb8888);

				// render for showing on the screen
				page.Render(bitmap, null, null, PdfRenderMode.ForDisplay);

				// add bitmaps to list for recycler
				pages.Add(bitmap);

				// close the page
				page.Close();

				if (pageCurrent >= pageCount)
				{
					break;
				}
			}

			// close the renderer
			renderer.Close();

			// if going back to previous pages we need to reverse the page list to keep the correct order
			if (!next)
			{
				pages.Reverse();
			}

			textPageCurrent.Text = "Viewing pages " + (pageCurrent - (pageGroupCount - 1)) + "-" + pageCurrent + " of " + pageCount;

			// set up recycler using bitmap list
			mAdapter.Pages = pages;
			mAdapter.NotifyDataSetChanged();
		}

		protected override void OnResume()
		{
			base.OnResume();

			// set orientation to portrait
			// If you start a portrait forced activity while holding your device in landscape mode, 
			// the activity will be recreated (destroyed and created again following the activity lifecycle).
			this.RequestedOrientation = ScreenOrientation.Portrait;
		}

		void ClearPages()
		{
			for (int i = 0; i < pages.Count; i++)
			{
				var image = pages[i];
				image.Dispose();
				image = null;
			}
			pages = null;
		}

		// Handler for the item click event:
		void OnItemClick(object sender, int position)
		{
			// Display a toast that briefly shows the enumeration of the selected pdf page:
			int pageNum = pageCurrent - (pageGroupCount - (position + 1));
			Toast.MakeText(this, "This is pdf page number " + pageNum, ToastLength.Short).Show();
		}

		void BtnPrev_Click(object sender, EventArgs e)
		{
			if (pageCurrent > pageMax)
			{
				RenderPages(false);
			}
		}

		void BtnNext_Click(object sender, EventArgs e)
		{
			if (pageCurrent < pageCount)
			{
				RenderPages();
			}
		}
	}
}
