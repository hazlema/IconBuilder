# IconBuilder
![alt text](https://raw.githubusercontent.com/hazlema/IconBuilder/master/IconWrite.png "Screenshot")<br>
Display or Convert 32bit  icons.<br>

View Mode<br>
&nbsp;&nbsp;&nbsp;* Display all resolutions of an icon<br>
&nbsp;&nbsp;&nbsp;* Associate explorer ico files for one click viewing<br>
&nbsp;&nbsp;&nbsp;* Drag and drop, Drop an ico file on the window to view<br>
&nbsp;&nbsp;&nbsp;* Renders HiRes 32bit Icons<br>
  
Import Mode<br>
&nbsp;&nbsp;&nbsp;* Auto scale & resize<br>
&nbsp;&nbsp;&nbsp;* Auto filename<br>
&nbsp;&nbsp;&nbsp;* Convert multiple files at once<br>
&nbsp;&nbsp;&nbsp;* Drag and drop, Drop multiple image files from explorer to batch convert<br>
&nbsp;&nbsp;&nbsp;* Renders HiRes 32bit Icons<br>

Code Sample View:<br>
&nbsp;&nbsp;&nbsp;// index starts with 0<br>
&nbsp;&nbsp;&nbsp;iconRedux icon = new iconRedux();<br>
&nbsp;&nbsp;&nbsp;icon.readFile("myico.ico");<br>
&nbsp;&nbsp;&nbsp;pic.image = icon.view(index); <br>
<br>
Code Sample Import:<br>
&nbsp;&nbsp;&nbsp;// Builds all sizes 128, 96, 72, 64, 48, 32, 24, 16<br>
&nbsp;&nbsp;&nbsp;iconRedux icon = new iconRedux();<br>
&nbsp;&nbsp;&nbsp;icon.build(new Bitmap("mypic.jpg"), "mypic.ico"); <br>
<br>
100% written in c#!
