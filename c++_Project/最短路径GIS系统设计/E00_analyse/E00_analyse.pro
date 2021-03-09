QT += gui
QT += widgets
QT += opengl

TEMPLATE = app
CONFIG += console c++11
#CONFIG -= app_bundle
#CONFIG -= qt
LIBS+= -framework opengl -framework glut

HEADERS += \
    carc.h \
    cnode.h \
    ce00.h \
    cfile.h \
    cexp.h \
    cproject.h \
    cmyopenglwidgt.h \
    mainwindow.h \
    ctol.h \
    clog.h \
    cattribution.h \
    cattrecord.h \
    csearch.h \
    cnodetable.h \
    cpoint.h

SOURCES += \
    carc.cpp \
    cnode.cpp \
    ce00.cpp \
    cfile.cpp \
    main.cpp \
    cexp.cpp \
    cproject.cpp \
    cmyopenglwidgt.cpp \
    mainwindow.cpp \
    ctol.cpp \
    clog.cpp \
    cattribution.cpp \
    cattrecord.cpp \
    csearch.cpp \
    cnodetable.cpp \
    cpoint.cpp

FORMS += \
    mainwindow.ui

win32:CONFIG(release, debug|release): LIBS += -L$$PWD/lib/release/ -lgdal.20
else:win32:CONFIG(debug, debug|release): LIBS += -L$$PWD/lib/debug/ -lgdal.20
else:unix: LIBS += -L$$PWD/lib/ -lgdal.20

INCLUDEPATH += $$PWD/include
DEPENDPATH += $$PWD/include
