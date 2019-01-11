lib_all : $(libs)/FoldBreak.dll \

$(libs)/FoldBreak.dll : lib/FoldBreak.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/FoldBreak.dll \
                       lib/FoldBreak.fs
