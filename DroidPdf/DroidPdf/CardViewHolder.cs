using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace DroidPdf
{
	public class CardViewHolder : RecyclerView.ViewHolder
	{
		public ImageView Image { get; private set; }

		// Get references to the views defined in the CardView layout.
		public CardViewHolder(View itemView, Action<int> listener) : base(itemView)
		{
			// Locate and cache view references:
			Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);

			// Detect user clicks on the item view and report which item
			// was clicked (by layout position) to the listener:
			itemView.Click += (sender, e) => listener(base.LayoutPosition);
		}
	}
}
