{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "tt-coupons.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "tt-coupons.fullname" -}}
{{- default .Release.Name .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "tt-coupons.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{- define "tt-coupons-constr" -}}
{{- $user := .Values.inf.db.coupons.user -}}
{{- $pwd := .Values.inf.db.coupons.pwd -}}
{{- $host := .Values.inf.db.coupons.host -}}
{{- $port := .Values.inf.db.coupons.port -}}
{{- $dbName := .Values.inf.db.coupons.dbName -}}
{{- $constr := printf "%s:%s@%s:%s/%s" $user $pwd $host $port $dbName -}}
{{- if .Values.inf.db.coupons.avoidSsl -}} 
{{- printf "%s" $constr -}}
{{- else -}}
{{- printf "%s?ssl=true" $constr -}}
{{- end -}}
{{- end -}}
