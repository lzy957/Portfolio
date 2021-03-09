#include "clistsymbol.h"

CListSymbol::CListSymbol()
{

}

CListSymbol::~CListSymbol()
{
    list<CSymbol*>::iterator i;
    for( i=this->Symbollist.begin();i!=this->Symbollist.end();++i)
        delete(*i);
}
