#!/bin/bash
RI=${1}
CORE_TYPE=${2}
CORE_TYPE=${CORE_TYPE:="latest"}

wget http://archives.izenda.us/core/${CORE_TYPE}/izenda.adhoc.zip
unzip izenda.adhoc.zip
rm izenda.adhoc.zip
mkdir -p Reports
touch Izenda.config
mkdir -p "${RI}"

sed -i '/<script type="text\/javascript" src=".\/rs.aspx?js_nocache=ModernScripts.IzendaLocalization"><\/script>/a\
\<script type="text\/javascript" src="annotation.js"><\/script>' Resources/html/ReportViewer-Head.ascx

line=$(grep -n 'rs.aspx?output=CSV' Resources/html/ReportViewer-Body.ascx | grep -o '^[0-9]*')
line=$((line - 2))
echo $line

sed -i "${line} {
  r powerpoint.html
}" Resources/html/ReportViewer-Body.ascx

sed -i '/InitializeViewer();/a\
\    RefreshAnnotations();' Resources/html/ReportViewer-Body.ascx

find . \( -path "./.git" -o -path "./Forms/elrte/.git" -o -path "./${RI}" \) -prune -o -print | cpio -mpvd "${RI}"
zip -r ${RI}.zip ${RI}
