fsc=fsharpc --nologo
optimize=--optimize --tailcall+ --crossoptimize+
debug=-g


../output/Resources.dll : binaries/Resources.dll binaries
	cp binaries/* ../output

binaries/Resources.dll : Properties/Resources.Designer.cs Properties/Resources.resx binaries Properties/Resources.Designer.cs
	dmcs -optimize -target:library  -resource:Properties/Resources.resx -r:System.Drawing.dll -out:binaries/Resources.dll Properties/Resources.Designer.cs 

clean:
	rm -rf binaries

binaries :
	mkdir binaries
