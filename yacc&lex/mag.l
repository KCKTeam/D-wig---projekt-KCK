%{
#include <stdio.h>
#include "mag_l.h"
extern int yylval;
int yywrap(void);
%}

%%


(P|p)odnie(ś|s)|(U|u)nie(ś|s)				return(CZASOWNIK_PODNOSZENIA);
(U|u)pu(ś|s)(ć|c)|(P|p)o(ł|l)(ó|o)(ż|z)		return(CZASOWNIK_OPUSZCZENIA);
kontener|beczk(ę|e)|samoch(o|ó)d 			return(RODZAJ);
czerwon[a-z]+|niebiesk[a-z]+|zielon[a-z]+	return(KOLOR);
\n 											return(0);

%%
int yywrap()
{
return(1);
}
