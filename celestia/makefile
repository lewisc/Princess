fsc=fsharpc --nologo --warn:5 -I:binaries -g
optimize=--optimize+ --tailcalls+ --crossoptimize+
debug=-g
bin=binaries
libs=binaries
folders=connection search board endgamedb players tables test lib


all  : IMCSIO_all \
       search_all \
       board_all \
       endgamedb_all \
       players_all \
       tables_all \
       lib_all \
       test_all 

include IMCSIO/build.mk \
        search/build.mk \
        board/build.mk \
        endgamedb/build.mk \
        players/build.mk \
        test/build.mk \
        lib/build.mk \
        tables/build.mk

lint : $(folders:=.lint)

$(folders:=.lint) :
			rm -f $(basename $@)/$@
			for file in `ls $(basename $@)/*.fs`; do \
				echo $$file >> $(basename $@)/$@; \
				dotnet-fsharplint -sf $$file >> $(basename $@)/$@; \
			done


clean :
	rm -rf binaries/*.dll
	rm -rf binaries/*.exe
