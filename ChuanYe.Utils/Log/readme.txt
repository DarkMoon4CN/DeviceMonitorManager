  
  1.configSections节点
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler,log4net" />

  2.configuration 节点
<log4net>
    <appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
      <param name="File" value="%logpath%\" />
      <param name="AppendToFile" value="true" />
      <param name="MaxSizeRollBackups" value="100" />
      <param name="StaticLogFileName" value="false" />
      <param name="DatePattern" value="&quot;&quot;yyyy-MM-dd&quot;.html&quot;" />
      <param name="RollingStyle" value="Date" />
      <layout type="log4net.Layout.PatternLayout">
        <param name="ConversionPattern" value="&lt;HR COLOR=red&gt;%n%-5p：%d [%t] &lt;BR&gt;%n%m &lt;BR&gt;%n  &lt;HR Size=1&gt;" />
      </layout>
      <threshold value="INFO" />
    </appender>
    <root>
      <level value="ALL" />
      <appender-ref ref="RollingLogFileAppender" />
    </root>
</log4net>