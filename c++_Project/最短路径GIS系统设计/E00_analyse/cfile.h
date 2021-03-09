#ifndef CFILE_H
#define CFILE_H
#include "ce00.h"
#include "cproject.h"
#include "csearch.h"

class CFile
{
public:
    CFile();
    void FileOpen(const char* filename);
    CE00 *E00;
    CSearch *search;
    CProject *prj;
};

#endif // CFILE_H
