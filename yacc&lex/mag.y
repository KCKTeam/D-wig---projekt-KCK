%{
#include <stdio.h>
extern char yytext[];
int yylex();
int yyerror(char *s);
%}
//Tutaj mamy tokeny jakie używamy
%token CZASOWNIK_PODNOSZENIA CZASOWNIK_OPUSZCZENIA RODZAJ KOLOR
%%
//Tutaj gramatyka naszego polecenia
cmd: czasownik przedmiot;
//Produkcje gramatyki
czasownik: CZASOWNIK_PODNOSZENIA|CZASOWNIK_OPUSZCZENIA;
przedmiot: cecha przedmiot|RODZAJ;
cecha: KOLOR;
%%
int yyerror(char *s)
{
fprintf(stderr, "%s at '%s'\n",s,(*yytext!='\n')? yytext: "\\n");
}
