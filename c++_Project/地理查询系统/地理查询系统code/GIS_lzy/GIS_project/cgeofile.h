#ifndef CGEOFILE_H
#define CGEOFILE_H
#include "cgeomap.h"
#include "cgeopoint.h"
#include "cgeopolyline.h"
#include "cgeopolygon.h"
#include <list>
#include "clistsymbol.h"
#include "ccitylist.h"
#include "cproject.h"
using namespace std;

class CGeoFile
{
public:
    CGeoFile();
    ~CGeoFile();
    void GraphicFileOpen(const char* filename);
    void SybFileOpen(const char* filename);
    CGeomap* map;
    CListSymbol* Symbolset;
    void CityFileOpen(const char* filename);
    CCityList Cityset;
    void MatchSymbol(CListSymbol* Symbolset);
    void FileOpen();
};

#endif // CGEOFILE_H
