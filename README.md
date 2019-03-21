PDF Rendering in Xamarin.Android
=====================

This project contains examples of rendering a PDF in a Xamarin.Android project using a `RecyclerView`.


### PdfFileActivity.cs

This is set up using a separate `PdfFile` class and renders the PDF page bitmaps on the fly allowing a very large PDF to be viewed with out issue.


### PdfPagingActivity.cs

This is set up to page through a PDF file. Only rendering a preset number of pages at a time also allows for viewing a large PDF with out issue.


### PdfBasicActivity.cs

This is set up to show basic PDF rendering where all the pages are rendered at once. If there are more pages than the device can handle the entire PDF will not be viewable, which can cause an Out Of Memory (OOM) error and crash the app.


### Switching examples

`PdfFileActivity.cs` is set as the opening screen. If you want to test one of the other examples simply uncomment the Activity label line containing `MainLauncher = true` at the top of that file (as shown below) and comment it out in the other two examples.

```
[Activity(Label = "PdfFileActivity", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
```

Hopefully these examples are helpful. Feel free to use them however you'd like.

Cheers!
