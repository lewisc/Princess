fsc=fsharpc --nologo --warn:5 -I:binaries
optimize=--optimize+ --tailcalls+ --crossoptimize+
debug=-g
bin=binaries
libs=binaries


all  : connection_all \
       search_all \
       board_all \
       endgamedb_all \
       players_all \
       tables_all \
       test_all 

include connection/build.mk \
        search/build.mk \
        board/build.mk \
        endgamedb/build.mk \
        players/build.mk \
        test/build.mk \
        tables/build.mk

clean :
	rm -rf binaries/*.dll
	rm -rf binaries/*.exe
