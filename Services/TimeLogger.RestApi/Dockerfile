FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
EXPOSE 80
EXPOSE 443

#Bitbucket build information
ARG ww_build_branch=develop
ARG ww_build_number=dev
ARG ww_build_commit=local
ARG ww_build_date=today

ENV WW_BUILD_NUMBER=$ww_build_number
ENV WW_BUILD_COMMIT=$ww_build_commit
ENV WW_BUILD_DATE=$ww_build_date
ENV WW_BUILD_BRANCH=$ww_build_branch


RUN mkdir app

COPY /bin/Release/ /app/
WORKDIR /app
ENTRYPOINT ["dotnet", "TimeLogger.RestApi.dll"]