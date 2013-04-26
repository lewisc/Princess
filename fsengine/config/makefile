fsc=fsharpc --nologo --warn:5 -I:binaries
optimize=--optimize --tailcall+ --crossoptimize+
debug=-g


all : binaries/Resources.dll binaries

binaries/Resources.dll : Properties/Resources.Designer.cs Properties/Resources.resx binaries Properties/Resources.Designer.cs
	dmcs -optimize -target:library  -resource:Properties/Config.Properties.Resources.resources -r:System.Drawing.dll -out:binaries/Resources.dll Properties/Resources.Designer.cs 

clean :
	rm -rf binaries

binaries :
	mkdir -p binaries
