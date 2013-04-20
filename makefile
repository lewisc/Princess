fsc=fsharpc --nologo
optimize=--optimize --tailcall+ --crossoptimize+
debug=-g

product : all
	$(fsc) $(optimize) --out:bin --standalone

output bin :
	mkdir output bin
