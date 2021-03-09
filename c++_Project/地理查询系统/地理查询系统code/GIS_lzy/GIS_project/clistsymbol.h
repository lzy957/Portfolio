#ifndef CLISTSYMBOL_H
#define CLISTSYMBOL_H
#include "list"
#include "csymbol.h"
using namespace std;

class CListSymbol
{
public:
    CListSymbol();
    ~CListSymbol();
    list<CSymbol*> Symbollist;
    int n;
};

#endif // CLISTSYMBOL_H
