#include <stdio.h>

int yyparse();
int main()
{
do
{
	fprintf(stdout,"?");
	if(yyparse())
		fprintf(stderr,"KO\n");
	else
		fprintf(stderr,"OK\n");
}
while(1);
return 0;
}
