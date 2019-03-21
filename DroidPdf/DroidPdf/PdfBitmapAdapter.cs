using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace DroidPdf
{
	// Adapter to connect the data set (bitmap list) to the RecyclerView:
	public class PdfBitmapAdapter : RecyclerView.Adapter
	{
		// Event handler for item clicks:
		public event EventHandler<int> ItemClick;

		public List<Bitmap> Pages { get; set; }

		// Load the adapter with the data set (pdf file) at construction time:
		public PdfBitmapAdapter(List<Bitmap> pages)
		{
			Pages = pages;
		}

		// Create a new pdf CardView (invoked by the layout manager): 
		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			// Inflate the CardView for the pdf:
			View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.card_view, parent, false);

			// Create a ViewHolder to find and hold these view references, and 
			// register OnClick with the view holder:
			CardViewHolder vh = new CardViewHolder(itemView, OnClick);

			return vh;
		}

		// Fill in the contents of the pdf card (invoked by the layout manager):
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			CardViewHolder vh = holder as CardViewHolder;

			// Set the ImageView and TextView in this ViewHolder's CardView 
			// from this position in the pdf file:
			vh.Image.SetImageBitmap(Pages[position]);
		}

		// Return the number of pdfs available in the pdf file:
		public override int ItemCount
		{
			get { return Pages.Count; }
		}

		// Raise an event when the item-click takes place:
		void OnClick(int position)
		{
			if (ItemClick != null)
				ItemClick(this, position);
		}
	}
}
